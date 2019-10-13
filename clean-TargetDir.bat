cd %1
for %%i in (*.*) do if not "%%i"==".gitignore" if not "%%1"==".git" del /q "%%i"