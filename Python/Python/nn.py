print("\nImporting TensorFlow...")
import tensorflow as tf
import tensorflow.contrib.slim as slim
print("TensorFlow imported")
import numpy as np

try:
    xrange = xrange
except:
    xrange = range

class ConvolutionalNeuralNetwork():

    def __init__(self):

        #Input image size
        self.img_x = 84
        self.img_y = 84
        self.img_channel = 3

        #Network Parameters 
        self.n_image = self.img_x * self.img_y  # data input (img shape: 84*84)
        self.n_classes = 26                     # total classes (actions)
        self.dropout = 0.75                     # Dropout, probability to keep units
        self.learning_rate=0.003                #Learning rate to update covnet

        #Placeholders

        #Covnet input
        #Input image (84x84x3)
        self.input_image = tf.placeholder(tf.float32, [None, self.img_x, self.img_y, self.img_channel])
        #Dropout (keep probability)
        self.keep_prob = tf.placeholder(tf.float32)
        #Obtained reward
        self.reward_holder = tf.placeholder(shape=[None],dtype=tf.float32)
        #Achieved action
        self.action_holder = tf.placeholder(shape=[None],dtype=tf.int32)
        #Sensors
        self.sensors = tf.placeholder(shape=[None,4],dtype=tf.float32)

        #Covnet output
        self.Q = tf.placeholder(tf.float32, [None, self.n_classes])

        #Store layers weight & bias
        self.weights = {
            # 8x8 conv, 1 input, 32 outputs
            'wc1': tf.Variable(tf.truncated_normal([8, 8, 3, 32], 0.0)),
            # 4x4 conv, 32 inputs, 64 outputs
            'wc2': tf.Variable(tf.truncated_normal([4, 4, 32, 64], 0.0)),
            # 3x3 conv, 64 inputs, 64 ones
            'wc3': tf.Variable(tf.truncated_normal([3, 3, 64, 64], 0.0)),
            # fully connected, 7*7*64 inputs, 512 outputs
            'wd1': tf.Variable(tf.truncated_normal([7*7*64, 512], 0.0)),
            # 516 inputs, 26 outputs (class prediction)
            'out': tf.Variable(tf.truncated_normal([516, self.n_classes]))
        }

        self.biases = {
            'bc1': tf.Variable(tf.ones([32])),
            'bc2': tf.Variable(tf.ones([64])),
            'bc3': tf.Variable(tf.ones([64])),
            'bd1': tf.Variable(tf.ones([512])),
            'out': tf.Variable(tf.zeros([self.n_classes]))
        }

        # Construct model

        #Normalizationm : 0 to 255 => -1 to 1
        self.input_image = tf.multiply(tf.subtract(tf.scalar_mul((1.0/255.0), self.input_image),0.5),2.0) 

        #Select action
        #Covnet
        self.Q = self.conv_net(self.input_image, self.weights, self.biases, self.keep_prob)
        #Softmax function
        self.soft = tf.nn.softmax(self.Q)
        #Maximum argument of softmax
        self.chosen_action = tf.argmax(self.soft,1) 

        """self.Q1 = tf.placeholder(shape=[1,1,None],dtype=tf.float32)
        self.loss = tf.reduce_sum(tf.square(self.Q1 - self.Q))
        optimizer = tf.train.AdamOptimizer(learning_rate=self.learning_rate)
        self.updateModel = optimizer.minimize(self.loss)"""

        #Update covnet
        #Responsible weight finding thanks to the achieved action
        self.responsible_weight = tf.slice(self.Q[0],self.action_holder,[1])
        #Loss calculation : -(log(responsible_weight) * reward_holder)
        self.log = tf.log(self.responsible_weight)
        self.loss = -(self.log*self.reward_holder)
        #Optimization
        optimizer = tf.train.AdamOptimizer(learning_rate=self.learning_rate)
        self.updateModel = optimizer.minimize(self.loss)

    # Create model
    def conv_net(self, x, weights, biases, dropout):

        #DeepMind Neural Network
        #https://www.intelnervana.com/demystifying-deep-reinforcement-learning/

        print("\nConvolutional neural network characteristic :")
        # Reshape input picture
        x = tf.reshape(x, shape=[-1, self.img_x, self.img_y, 3])
        print("Input      : ", x._shape)

        # Convolution Layer
        conv1 = conv2d(x, weights['wc1'], biases['bc1'], 4)
        print("Conv 1     : ", conv1._shape)
        
        # Convolution Layer
        conv2 = conv2d(conv1, weights['wc2'], biases['bc2'], 2)
        print("Conv 2     : ", conv2._shape)

        # Convolution Layer
        conv3 = conv2d(conv2, weights['wc3'], biases['bc3'], 1)
        print("Conv 3     : ", conv3._shape)

        # Fully connected layer
        # Reshape conv2 output to fit fully connected layer input
        fc1 = tf.reshape(conv3, [-1, weights['wd1'].get_shape().as_list()[0]])
        print("Reshape    : ", fc1.shape)
        fc1 = tf.add(tf.matmul(fc1, weights['wd1']), biases['bd1'])
        print("Calculate  : ", fc1.shape)
        fc1 = tf.nn.relu(fc1)
        print("Relu       : ", fc1.shape)
        # Apply Dropout
        fc1 = tf.nn.dropout(fc1, dropout)
        print("Dropout    : ", fc1.shape)
        fc1 = tf.concat([fc1,self.sensors],1)
        print("Merge      : ", fc1.shape)
        # Output, class prediction
        out = tf.add(tf.matmul(fc1, weights['out']), biases['out'])
        print("Out        : ", out.shape)
        print("\n")
        return out
        

# Create some wrappers for simplicity
def conv2d(x, W, b, strides):
    # Conv2D wrapper, with bias and relu activation
    x = tf.nn.conv2d(x, W, strides=[1, strides, strides, 1], padding='VALID')
    x = tf.nn.bias_add(x, b)    
    return tf.nn.relu(x)
