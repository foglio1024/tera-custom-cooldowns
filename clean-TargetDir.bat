cd %1
for %%i in (*.*) do if not "%%i"==".gitignore" if not "%%i"==".git" del /q "%%i"