from platform_utils	import on_windows
from platform_utils	import on_linux

if on_windows():
	from win32console import PyINPUT_RECORDType
	from win32console import KEY_EVENT
	from win32console import GetStdHandle
	from win32console import STD_INPUT_HANDLE
elif on_linux():
	from readline import set_startup_hook
	from readline import insert_text

# prompt user with default response
def prompt(prompt, default=''):
	if on_windows():
		keys = []
		for c in str(default):
			eve = PyINPUT_RECORDType(KEY_EVENT)
			eve.Char = c
			eve.RepeatCount = 1
			eve.KeyDown = True
			keys.append(eve)
		std_input = GetStdHandle(STD_INPUT_HANDLE)
		std_input.WriteConsoleInput(keys)
	elif on_linux():
		set_startup_hook(lambda : insert_text(default))
		try:
			return input(prompt)
		finally:
			set_startup_hook()
	return input(prompt)
