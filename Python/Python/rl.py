from env import *
import random
from nn import *
from plot import *
from console import prompt
from time import *

def train():

    #Simulation parameters
    total_episodes = 1000
    total_step = 30
    best_episode_reward = 0.0
    best_reward = -1000
    worst_reward = 1000
    avg_reward = 0.0
    e = 0.1
    pixel_done = False
    reset_crash = False
    nb_pixel_crash = 0

    #Robot environement creation
    env = EnvRobot(total_step)
    print("\n\n=========================== RL PROGRAM ===========================")

    #TensorFlow graph
    tf.reset_default_graph() 
    cnn = ConvolutionalNeuralNetwork() #Load the convolutional neural network
    init = tf.global_variables_initializer()

    #To save the Tensorflow graph
    saver = tf.train.Saver()
    restore_destination = "save/checkpoint/restore/"
    saver_destination = find_folder("save/checkpoint/trained/")

    #Plot parameters
    plot_reward = np.empty([total_episodes])
    plot_avg_reward = np.empty([total_episodes])
    print(saver_destination.split("/")[-2])
    fig = PlotReward(saver_destination.split("/")[-2])
    
    #Launch the graph
    with tf.Session() as sess:
        sess.run(init)

        #Restore a model
        restore = prompt('\nRestore session (y/ n): ', 'y')
        if restore == 'y':
            saver.restore(sess, restore_destination + "model.ckpt")
            print("The model was successfully restored !")
        else:
            print("No model has been restored !")

        time_start = time() 
        #Episode loop
        for x in range(total_episodes):

            #Reset the environement
            reset_cube = pixel_done or reset_crash
            state = env.res(reset_cube=reset_cube)
            reset_crash = False
            done = False
            running_reward = 0

            #Step loop
            for i in range(total_step):

                print("\n--------------------------- Step %d ---------------------------" % (i+1))

                #Choose action from camera with the covnet
                #input_image    : camera
                #keep_prob      : dropout (if 0.75, 75% chance to keep the neuron)
                #Q              : output of the last fully connected layer of the covnet
                #soft           : softmax function from Q values
                #action         : chosen action
                Q, soft, action = sess.run([cnn.Q, cnn.soft, cnn.chosen_action], feed_dict={cnn.input_image:[state[0]], cnn.sensors:[state[1]], cnn.keep_prob:[cnn.dropout]})
                """print("\nPrediction Q     : %0.3f  %0.3f  %0.3f  %0.3f  %0.3f  %0.3f" % (Q[0][0], Q[0][1], Q[0][2], Q[0][3], Q[0][4], Q[0][5]))
                print("Softmax          : %d %d %d %d %d %d" % (soft[0][0], soft[0][1], soft[0][2], soft[0][3], soft[0][4], soft[0][5]))"""

                #Action choice (with e chance of random action)
                rdm = np.random.rand(1)
                if  rdm < e:
                    action = random.randint(0, cnn.n_classes-1)
                    print("Random action    : %d (%s)" % (action, action_name(action)))    
                else:
                    action = action[0]
                    print("Chosen action    : %d (%s)" % (action, action_name(action)))

                #Step with the action + informations recovery
                #action     : chosen action
                #state1     : next state
                #reward     : reward value
                #done       : true if the simulation is finished 
                #pixel_done : true if the camera is composed of only red pixels
                state1, reward, done, pixel_done = env.step(action)

                """Q1 = sess.run([cnn.Q], feed_dict={cnn.input_image:[state1], cnn.keep_prob:[cnn.dropout]})
                maxQ1 = np.max(Q1)
                targetQ = Q
                targetQ[0,action] = reward + 0.99*maxQ1
                _, loss = sess.run([cnn.updateModel, cnn.loss], feed_dict={cnn.input_image:[state], cnn.keep_prob:[cnn.dropout], cnn.Q1:[targetQ]})"""

                #Update the covnet
                #input_image    : camera
                #keep_prob      : dropout (if 0.75, 75% chance to keep the neuron)
                #reward_holder  : obtained reward
                #action_holder  : achieved action
                #resp           : responsible weight
                #log            : log(resp)
                #loss           : calculated loss => -(log(resp) * reward_holder)
                _,resp, log, loss = sess.run([cnn.updateModel,cnn.responsible_weight, cnn.log, cnn.loss], feed_dict={cnn.input_image:[state[0]], cnn.sensors:[state[1]], cnn.keep_prob:[cnn.dropout], cnn.reward_holder:[reward], cnn.action_holder:[action]})

                """print("\nCalulation")
                print("Resp     : ", resp)
                print("Log      : ", log)
                print("A        : ", reward)
                print("Loss     : ", loss)"""

                #Update the state
                state = state1

                #Display the current reward
                print("\nCurrent reward : %.2f" % reward)

                #Best reward calculation
                running_reward += reward
                if running_reward > best_episode_reward:
                        best_episode_reward = running_reward

                if done:
                    #Episode finished

                    #Change red cube position after n fails
                    if pixel_done:
                        nb_pixel_crash = 0
                    else:
                        nb_pixel_crash += 1 

                    if nb_pixel_crash == 10:
                        reset_crash = True
                    break

            time_end = time()
            #Display episode informations
            print("\n\n                           Time = %.2fs" % (time_end - time_start))
            print("=========================== EPISODE : %d ===========================" % (x+1))
            print("REWARD       = %.2f" % running_reward)
            print("BEST REWARD  = %.2f" % best_episode_reward)

            #Training reward calculation
            avg_reward += running_reward
            if running_reward > best_reward:
                best_reward = running_reward
            if running_reward < worst_reward:
                worst_reward = running_reward

            #Plot rewards and save the graph
            plot_reward[x] = running_reward
            plot_avg_reward[x] = avg_reward/(x+1) 
            fig.update(plot_reward, x, best_reward, worst_reward, plot_avg_reward, done, pixel_done)

            # Save the variables to disk.
            if x % 100 == 0 and x != 0:
                save_path = saver.save(sess, saver_destination + "model.ckpt")
                print("Model saved in file: %s" % save_path)
         
        #Display training informations
        print("\n======================== TRAINING FINISHED ========================")
        print("AVG REWARD           = %.2f" % (avg_reward/total_episodes))
        print("BEST EPISODE REWARD  = %.2f" % best_reward)


#Find the name of the folder for saving model + create folder: simulation(?)
def find_folder(destination):
    done = False
    i=0
    while(not(done)):
        if os.path.isdir(destination + 'simulation(' + str(i) + ')'):
            i += 1
        else:
            os.makedirs(destination + 'simulation(' + str(i) + ')')
            path = destination + 'simulation(' + str(i) + ')/'
            done = True
    return path