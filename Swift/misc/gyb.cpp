// gyb.cpp : 콘솔 응용 프로그램에 대한 진입점을 정의합니다.
//
#include <stdio.h>
#include <string>

int main(int argc, char **argv)
{
	const char *python_dir = getenv("GYB_PYTHON_DIR");
	if (python_dir == NULL)
		python_dir = "C:\\python27";

	std::string cmd = python_dir;
	cmd += "\\python.exe";

	for (int i = 0; i < argc; i++)
	{
		cmd += " ";
		cmd += argv[i];
	}

	system((char *)cmd.c_str());
    return 0;
}

