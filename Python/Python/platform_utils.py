from platform import system

def on_windows():
	return system() == 'Windows'

def on_linux():
	return system() == 'Linux'