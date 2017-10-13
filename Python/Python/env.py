from tcp import tcp_server
from server import *
from PIL import *
import numpy as np

class EnvRobot():

    def __init__(self, total_step):

        # start server
        self.server = tcp_server()
        ip, port = get_server_settings()
        self.server.connect(ip, port)
    
        #Attributes initialization

        #For interaction with the server
        self.base               = 0.0
        self.lnk1               = 69.0
        self.lnk2               = -85.0
        self.time               = 1.0
        self.reset              = False
        self.camera             = None
        self.actual_position    = None
        self.target_position    = None
        
        #Camera size
        self.camera_width = 256
        self.camera_height = 256
        
        #Sensors
        self.sensor_up          = 0.0
        self.sensor_down        = 0.0
        self.sensor_left        = 0.0
        self.base_left          = 0.0
        self.sensor_right       = 0.0
        self.base_right         = 0.0

        #Simulation parameters
        self.step_size          = 4
        self.angle              = 20
        self.height_max         = 21.0
        self.height_min         = 7.0
        self.time_size          = 0.000001
        self.total_step         = total_step
        self.reward_step        = -0.5
        self.reward_win         = 200 
        self.reward_lose        = -200
        self.reward_pixel_max   = 30.0

    def step(self, action):

        #Robot data reception from the server
        data_from_server = from_server(self.server)
        self.camera          = data_from_server[0]
        self.actual_position = data_from_server[1]
        self.target_position = data_from_server[2]
        self.base            = data_from_server[3]
        self.lnk1            = data_from_server[4]
        self.lnk2            = data_from_server[5]
        self.time = self.time_size
        self.reset = False

        #Calculate the new position from the action
        self.base, self.lnk1, self.lnk2 = do_action(action, self.base, self.lnk1, self.lnk2, self.step_size)

        #Send the action to the server
        data_to_server = (0.0,0.0,0.0,0.0,False)
        data_to_server = list(data_to_server)
        data_to_server[0] = self.time
        data_to_server[1] = self.base   
        data_to_server[2] = self.lnk1   
        data_to_server[3] = self.lnk2
        data_to_server[4] = self.reset
        data_to_server = tuple(data_to_server)
        to_server(self.server,data_to_server)
        

        #Update attributes from the server
        data_from_server = from_server(self.server)
        self.camera          = data_from_server[0]
        self.actual_position = data_from_server[1]
        self.target_position = data_from_server[2]
        self.base            = data_from_server[3]
        self.lnk1            = data_from_server[4]
        self.lnk2            = data_from_server[5]
        self.time = self.time_size
        self.reset = False
        data_to_server = (0.0,0.0,0.0,0.0,False)
        data_to_server = list(data_to_server)
        data_to_server[0] = 0.0
        data_to_server[1] = self.base   
        data_to_server[2] = self.lnk1   
        data_to_server[3] = self.lnk2
        data_to_server[4] = self.reset
        data_to_server = tuple(data_to_server)
        to_server(self.server,data_to_server)

        #Observations
        #Virtual sensors (Robot point of view)
        self.sensor_up = self.height_max - self.actual_position.y
        self.sensor_down = self.actual_position.y - self.height_min
        if self.base <= 180:
            self.sensor_left = self.angle - self.base
            self.sensor_right = self.angle + self.base
        else:
            self.sensor_left = self.angle + (360 - self.base)
            self.sensor_right = self.base - (360 - self.angle)

        
        #Camera resizing for covnet
        img = np.array(self.camera.resize((84,84), Image.ANTIALIAS))
        sensors = np.array([self.sensor_up, self.sensor_down, self.sensor_left, self.sensor_right])
        state = np.array([img, sensors])

        """print("Sensor up    : ", self.sensor_up)
        print("Sensor down  : ", self.sensor_down)
        print("Sensor left  : ", self.sensor_left)
        print("Sensor right : ", self.sensor_right)"""

        #Done condition

        #Movement
        left_done = bool(self.sensor_left <= -self.step_size)
        right_done = bool(self.sensor_right <= -self.step_size)
        up_done = bool(self.sensor_up <= -1)
        down_done = bool(self.sensor_down <= -1)
        lnk1_done = bool(self.lnk1 > 90.0)

        #Camera pixel
        red_pixel_camera = red_pixel(self.camera)                   #0 to 65536
        red_pixel_camera /= self.camera_width * self.camera_height  #0 to 1
        red_pixel_camera *= self.reward_pixel_max                   #ex: 0 to 30
        """red_pixel_camera -= 0.5                                  #-0.5 to 0.5
        red_pixel_camera *= self.reward_pixel_max*2                 #ex : -30 to 30"""
        pixel_done = bool(red_pixel_camera == self.reward_pixel_max)

        done = left_done or right_done or up_done or down_done or lnk1_done or pixel_done 

        if not done:
            #Continue

            #Reward calculation
            reward = self.reward_step + red_pixel_camera
        else:
            #Finish
            if pixel_done:
                #Achived goal
                reward = self.reward_win
            else:
                #Fail / Crash
                reward = self.reward_lose

        return state, reward, done, pixel_done

    def res(self, reset_cube=False):

        #Reset function
        self.base               = 0.0
        self.lnk1               = 69.0
        self.lnk2               = -85.0
        self.time               = 0.00001
        self.reset              = reset_cube
        self.camera             = None
        self.actual_position    = None
        self.target_position    = None
        data_to_server = (0.0,0.0,0.0,0.0,False)
        data_to_server = list(data_to_server)
        data_to_server[0] = self.time
        data_to_server[1] = self.base   
        data_to_server[2] = self.lnk1   
        data_to_server[3] = self.lnk2
        data_to_server[4] = self.reset
        data_to_server = tuple(data_to_server)
        to_server(self.server,data_to_server)
        data_from_server = from_server(self.server)

        self.camera          = data_from_server[0]
        self.actual_position = data_from_server[1]
        self.target_position = data_from_server[2]
        self.base            = data_from_server[3]
        self.lnk1            = data_from_server[4]
        self.lnk2            = data_from_server[5]
        self.time = 1.0
        self.reset = False

        img = np.array(self.camera.resize((84,84), Image.ANTIALIAS))
        sensors = np.array([0,0,0,0])
        state = np.array([img, sensors])

        return state


def red_pixel(img):
    #Get color pixel of img
    red = 0
    for r, g, b in img.getdata():
        if r > 150:
            red += 1
    print("Red pixel        : %d / %d (%0.2f%%)" % (red, 256*256, (red/(256*256))*100))
    return red


def do_action(action, base, lnk1, lnk2, step):
 
    if action == 0:
        base += step #Base+
    elif action == 1:
        base -= step #Base-
    elif action == 2:        
        lnk1 += step #Lnk1+
    elif action == 3:
        lnk1 -= step #Lnk1-
    elif action == 4:
        lnk2 += step #Lnk2+
    elif action == 5:
        lnk2 -= step #Lnk2-
    elif action == 6:
        base += step #Base+,Lnk1+
        lnk1 += step
    elif action == 7:
        base += step #Base+,Lnk1-
        lnk1 -= step
    elif action == 8:
        base -= step #Base-,Lnk1+
        lnk1 += step
    elif action == 9:
        base -= step #Base-,Lnk1-
        lnk1 -= step
    elif action == 10:
        base += step #Base+,Lnk2+
        lnk2 += step
    elif action == 11:
        base += step #Base+,Lnk2-
        lnk2 -= step
    elif action == 12:
        base -= step #Base-,Lnk2+
        lnk2 += step
    elif action == 13:
        base -= step #Base-,Lnk2-
        lnk2 -= step
    elif action == 14:
        lnk1 += step #Lnk1+, Lnk2+
        lnk2 += step
    elif action == 15:
        lnk1 += step #Lnk1+, Lnk2-
        lnk2 -= step
    elif action == 16:
        lnk1 -= step #Lnk1-, Lnk2+
        lnk2 += step
    elif action == 17:
        lnk1 -= step #Lnk1-, Lnk2-
        lnk2 -= step
    elif action == 18:
        base += step # Base+, Lnk1+, Lnk2+
        lnk1 += step
        lnk2 += step
    elif action == 19:
        base += step # Base+, Lnk1+, Lnk2-
        lnk1 += step
        lnk2 -= step
    elif action == 20:
        base += step # Base+, Lnk1-, Lnk2+
        lnk1 -= step
        lnk2 += step
    elif action == 21:
        base += step # Base+, Lnk1-, Lnk2-
        lnk1 -= step
        lnk2 -= step
    elif action == 22:
        base -= step # Base-, Lnk1+, Lnk2+
        lnk1 += step
        lnk2 += step
    elif action == 23:
        base -= step # Base-, Lnk1+, Lnk2-
        lnk1 += step
        lnk2 -= step
    elif action == 24:
        base -= step # Base-, Lnk1-, Lnk2+
        lnk1 -= step
        lnk2 += step
    elif action == 25:
        base -= step # Base-, Lnk1-, Lnk2-
        lnk1 -= step
        lnk2 -= step
    else:
        print("[ERROR] The action %d doesn't exist" % action)

    return base, lnk1, lnk2


def action_name(action):

    if action == 0:
        return "Base+"
    elif action == 1:
        return "Base-"
    elif action == 2:
        return "Lnk1+"
    elif action == 3:
        return "Lnk1-"
    elif action == 4:
        return "Lnk2+"
    elif action == 5:
        return "Lnk2-"
    elif action == 6:
        return "Base+, Lnk1+"
    elif action == 7:
        return "Base+, Lnk1-"
    elif action == 8:
        return "Base-, Lnk1+"
    elif action == 9:
        return "Base-, Lnk1-"
    elif action == 10:
        return "Base+, Lnk2+"
    elif action == 11:
        return "Base+, Lnk2-"
    elif action == 12:
        return "Base-, Lnk2+"
    elif action == 13:
        return "Base-, Lnk2-"
    elif action == 14:
        return "Lnk1+, Lnk2+"
    elif action == 15:
        return "Lnk1+, Lnk2-"
    elif action == 16:
        return "Lnk1-, Lnk2+"
    elif action == 17:
        return "Lnk1-, Lnk2-"
    elif action == 18:
        return "Base+, Lnk1+, Lnk2+"
    elif action == 19:
        return "Base+, Lnk1+, Lnk2-"
    elif action == 20:
        return "Base+, Lnk1-, Lnk2+"
    elif action == 21:
        return "Base+, Lnk1-, Lnk2-"
    elif action == 22:
        return "Base-, Lnk1+, Lnk2+"
    elif action == 23:
        return "Base-, Lnk1+, Lnk2-"
    elif action == 24:
        return "Base-, Lnk1-, Lnk2+"
    elif action == 25:
        return "Base-, Lnk1-, Lnk2-"
    else:
        return "ERROR"