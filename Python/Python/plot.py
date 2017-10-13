import os
import matplotlib.pyplot as plt

#Disable some plot warning
import warnings
import matplotlib.cbook
warnings.filterwarnings("ignore",category=matplotlib.cbook.mplDeprecation)

class PlotReward():

    def __init__(self, simulation_name):

        self.simulation_name = simulation_name
        #Abcissa
        self.xdata = []
        #Ordinates
        self.ydata = []
        self.y2data = []
        self.y3data = []
        self.y4data = []
        self.y5data = []

        #Initialization
        self.nb_done = 0
        self.nb_crash = 0
        self.nb_fail = 0
        self.nb_episode = 0
        self.destination = "save/plot/"
        self.nb_filename = find_name(self.destination)

        #Create plot windows
        plt.figure(figsize=(8,7), num=self.simulation_name)
        
        #Add a suptittle
        plt.suptitle(self.simulation_name, fontsize=14, fontweight='bold')
        
        #Select windows
        self.axes_reward = plt.gca()
        #Shrink graph size for the legend
        box = self.axes_reward.get_position()
        self.axes_reward.set_position([box.x0, box.y0, box.width * 0.8, box.height])
        self.axes_reward.set_title('Reward for each episode')
        #Abcissa & ordinates labels
        self.axes_reward.set_xlabel('Episode')
        self.axes_reward.set_ylabel('Reward ')
        #Create curves 
        self.line, = self.axes_reward.plot(self.xdata, self.ydata, color='silver', label='Reward')
        self.line2, = self.axes_reward.plot(self.xdata, self.y2data, linestyle='dotted', color='black', label='Average')
        self.point, = self.axes_reward.plot(self.xdata, self.y3data, linestyle='None', marker='o', markerfacecolor='g', markeredgewidth=0.0, label='Done')
        self.point2, = self.axes_reward.plot(self.xdata, self.y4data, linestyle='None', marker='o', markerfacecolor='r', markeredgewidth=0.0, label='Crash')
        self.point3, = self.axes_reward.plot(self.xdata, self.y5data, linestyle='None', marker='o', markerfacecolor='orange', markeredgewidth=0.0, label='Fail')


    #Update the graph
    def update(self, data, episode, best_reward, worst_reward, avg_reward, done, pixel_done): #, step, total_step):

        #Add values to the graph
        self.xdata.append(episode)
        self.ydata.append(data[episode])
        self.y2data.append(avg_reward[episode])

        #Add value for "Done" or "Crash" or "Fail"
        if pixel_done:
            self.y3data.append(data[episode])
            self.nb_done += 1
        else:
             self.y3data.append(None)
        if done and not(pixel_done):
            self.y4data.append(data[episode])
            self.nb_crash += 1
        else:
             self.y4data.append(None)
        if not(done) and not(pixel_done):
            self.y5data.append(data[episode])
            self.nb_fail += 1
        else:
             self.y5data.append(None)


        #Counting the number of episodes
        self.nb_episode += 1   

        #Update graph values
        self.line.set_xdata(self.xdata)
        self.line.set_ydata(self.ydata)
        self.line2.set_xdata(self.xdata)
        self.line2.set_ydata(self.y2data)
        self.point.set_xdata(self.xdata)
        self.point.set_ydata(self.y3data)
        self.point2.set_xdata(self.xdata)
        self.point2.set_ydata(self.y4data)
        self.point3.set_xdata(self.xdata)
        self.point3.set_ydata(self.y5data)

        #Update the limit of the graf in function of graph values
        self.axes_reward.set_xlim(0, episode+1)
        self.axes_reward.set_ylim(worst_reward - 10, best_reward + 10)

        #Update legend
        plt.legend([self.line, self.line2, self.point, self.point3, self.point2], ['Reward', 'Average', 'Done (%.2f%%)' % ((self.nb_done/self.nb_episode)*100), 'Fail (%.2f%%)' % ((self.nb_fail/self.nb_episode)*100), 'Crash (%.2f%%)' % ((self.nb_crash/self.nb_episode)*100)], loc='center left', bbox_to_anchor=(1, 0.5))
        
        plt.draw()
        #Save the graph
        plt.savefig(self.destination + 'reward('+ self.nb_filename +').png')
        plt.pause(0.05)


#Find the name of the plot for saving : reward(?).png
def find_name(destination):
    done = False
    i=0
    while(not(done)):
        if os.path.isfile(destination + 'reward(' + str(i) + ').png'):
            i += 1
        else:
            done = True
    return str(i)