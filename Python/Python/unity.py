from console	import prompt
from math		import sqrt
from os			import getcwd
from subprocess	import Popen

from platform_utils	import on_linux
from platform_utils	import on_windows

# python implementation of unity's quaternion struct
class quaternion:
	def __init__(self, x, y, z, w):
		self.x = x;
		self.y = y;
		self.z = z;
		self.w = w;

	# to string
	def __str__(self, **kwargs):
		return '({0:.2f}, {1:.2f}, {2:.2f}, {3:.2f})'.format(self.x, self.y, self.z, self.w)
	
# python implementation of unity's vector3 struct
class vector3:
	def __init__(self, x, y, z):
		self.x = x
		self.y = y
		self.z = z

	# magnitude of vector
	def magnitude(self):
		return sqrt(
			self.x * self.x + 
			self.y * self.y +
			self.z * self.z
		)
	
	# to string
	def __str__(self, **kwargs):
		return '({0:.2f}, {1:.2f}, {2:.2f})'.format(self.x, self.y, self.z)

# prompt user to open unity environment
def open_unity_prompt():
    print("\n\n=========================== CONNECTION ===========================")
    opened = False
    while not opened:
        opened = True
        openUnityEnv = prompt('\nOpen Unity environment (y/n): ', 'y')
        if openUnityEnv == 'y':
            cwd = getcwd()
            if on_linux():
                foldersToFile = cwd[0 : len(cwd) - len(r'/Python/Python')] + r'/Unity/Build/'
                fileName = []
                fileName = prompt('Name of the built executable: ', fileName)
                unityEnvPath = foldersToFile + fileName
            elif on_windows():
                foldersToFile=cwd[0: len(cwd)-len(r'\Python\Python')] + r'\Unity\Build\\'
                fileName = "build.exe"
                fileName = prompt('Name of the built executable: ', fileName)
                unityEnvPath = foldersToFile + fileName
            try:
                man = prompt("Use manual position (y/n): ", 'n')
                Popen([unityEnvPath])
                return man
            except:
                opened = False