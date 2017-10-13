from os import system, environ

system("cls")
print("\n                                  ________________________")
print("                                 |                        |")
print("================================>|  ROBOT ARM SIMULATION  |<==============================")
print("                                 |________________________|")

print("\n\n __________________________________/!\ INFORMATION /!\__________________________________")
print("|                                                                                       |")
print("| The program doesn't work when it's launched from Visual Studio. Please, use Terminal. |")
print("|_______________________________________________________________________________________|")

#Disable TensorFlow warnings
environ['TF_CPP_MIN_LOG_LEVEL']='2'

from unity import open_unity_prompt
from rl import *
from manual import *

# main
def main():

    # prompt user to open unity
    man = open_unity_prompt()

    if man == 'y':
        manual_position()
    else:
        train()

if __name__ == '__main__':
	main()
