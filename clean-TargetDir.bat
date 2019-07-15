cd %1
for %%i in (%1*) do if not "%%1"==".git" del /q "%%i"