#include "InputParser.h"

#include <assert.h> 
#include <windows.h>
#include <iostream>
#include <fstream>
#include <string>
#include <vector>
#include <sstream>


using namespace std;

InputParser::InputParser(char* fileName)
{
	ifstream inStream;
	stringstream sStream;
	string line;
	float x, y, z;

	labels = std::vector < std::string >() ;
	points = std::vector<float> ();
	numFrames = 0;

	inStream.open(fileName, std::ifstream::in);

	if ((inStream.rdstate() & std::ifstream::failbit) != 0){
		std::cout << "Failed to open file.";
		exit(0);
	}

	getline(inStream, line);
	_split_labels(line, &labels);
	getline(inStream, line);

	int pointc = 0;

	while ((inStream.rdstate() & std::ifstream::eofbit) != 1)
	{
		if (line.front() != '#')
		{

			sStream = stringstream(line);
			std::string temp;

			std::getline(sStream, temp, ' ');
			std::stringstream(temp) >> x;
			std::getline(sStream, temp, ' ');
			std::stringstream(temp) >> y;
			std::getline(sStream, temp, ' ');
			std::stringstream(temp) >> z;
			points.push_back(x);
			points.push_back(y);
			points.push_back(z);

			pointc += 1;

		}
		else if (line.front() == '#')
		{
			numFrames += 1;
		}
		getline(inStream, line);


	}


	assert(points.size() == numFrames * labels.size() * 3);

	inStream.close();
}


void InputParser::_split_labels(string line, std::vector<string>* split )
{
	std::string token;

	line = line.substr(0, line.size() - 1);
	std::istringstream ss(line);
	while (getline(ss, token, ',')){
		if (token.front() == ' ')
			token = token.substr(1, token.size());
		split->push_back(token);
	}
}
