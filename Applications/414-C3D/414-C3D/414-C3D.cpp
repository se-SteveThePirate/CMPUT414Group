// 414-C3D.cpp : Defines the entry point for the console application.
//


#include "stdafx.h"
#include "Objbase.h" 
#include "InputParser.h"
#include <stdio.h>
#include <iostream> 
#include <sstream>

#import "c3dserver.dll"

using namespace std;


int main(int argc, char* argv[])
{
	CoInitialize(NULL);
	{

	C3DSERVERLib::IC3DPtr p(_uuidof(C3DSERVERLib::C3D));

	try{

	if (argc != 2){
		std::cout << "Filename must be given as argument.\n";
		exit(-1);
	}

	InputParser ip = InputParser(argv[1]);
	float *x, *y, *z, **channelsx, **channelsy, **channelsz;
	int count;


#ifdef _DEBUG
	std::cout << "Labels:\n\n";
	for (std::vector<std::string>::iterator it = ip.labels.begin(); it != ip.labels.end(); ++it) {
		std::cout << *it << '\n';
		Sleep(10);
	}

	std::cout << "Points:\n\n";
	for (std::vector<float>::iterator it = ip.points.begin(); it != ip.points.end(); it = it + 3) {
		std::cout << *it << " " << *(it + 1) << " " << *(it + 2) << "\n";
		Sleep(10);
	}
#endif

	const wchar_t* sFile = L"C:\\Users\\Owner\\Desktop\\414\\414-C3D\\Debug\\output.c3d";
	_bstr_t bStrFile = _bstr_t(sFile);


	p->NewFile(bStrFile, // File name
					  1, // File type = intel
					  2, // Data type is floating point
					  0, // 0 analog channels
					  3, // 0 analog frames per point frame
					  ip.labels.size(), // As many markers as labels
					  30, // 30 FPS
					  0.15, // Scaleing of points is 1
					  ip.numFrames); // Total number of frames


	p->SaveFile(bStrFile, 1);

	p->Open(bStrFile, 3);

	count = ip.numFrames * ip.labels.size();

	x = new float[count];
	y = new float[count];
	z = new float[count];

	channelsx = new float*[ip.labels.size()];
	channelsy = new float*[ip.labels.size()];
	channelsz = new float*[ip.labels.size()];

	for (int i = 0; i < ip.labels.size(); i++)
	{
		channelsx[i] = new float[ip.numFrames];
		channelsy[i] = new float[ip.numFrames];
		channelsz[i] = new float[ip.numFrames];
	}

	for (int i = 0; i < count; i++) 
	{
		x[i] = ip.points[3 * i]*1000;
		y[i] = ip.points[3 * i + 1]*1000;
		z[i] = ip.points[3 * i + 2]*1000;

	}

	for (int i = 0; i < ip.labels.size(); i++){
		for (int j = 0; j < ip.numFrames; j++){
			channelsx[i][j] = x[j*ip.labels.size() + i];
			channelsy[i][j] = y[j*ip.labels.size()+i];
			channelsz[i][j] = z[j*ip.labels.size()+i];
		}
	}

#ifdef _DEBUG

	std::cout << "Streams: \n\n";

	for (int i = 0; i < ip.labels.size(); i++){
		std::cout << "Label: " << ip.labels[i];

		std::cout << "\n X:";
		for (int j = 0; j < ip.numFrames; j++){
			std::cout << " " << channelsx[i][j] << "\n";
		}
		std::cout << "\n Y:";
		for (int j = 0; j < ip.numFrames; j++){
			std::cout << " " << channelsy[i][j] << "\n";
		}
		std::cout << "\n Z:";
		for (int j = 0; j < ip.numFrames; j++){
			std::cout << " " << channelsz[i][j] << "\n";
		}
		std::cout << "\n";
		}
#endif



	_variant_t vx, vy, vz, r, label, dim;

	r = 0;

	for (int i = 0; i < ip.labels.size(); i++){
		for (int j = 0; j < ip.numFrames; j++){
			vx = channelsx[i][j];
			vy = channelsy[i][j];
			vz = channelsz[i][j];

			p->SetPointData(i, 0, j+1, vx);
			p->SetPointData(i, 1, j+1, vy);
			p->SetPointData(i, 2, j+1, vz);
			p->SetPointData(i, 3, j + 1, r);

		}
	}
	
	HRESULT result = p->GetParameterIndex("POINT", "LABELS");
	p->DeleteParameter(result);


	dim.vt = VT_ARRAY;

	SAFEARRAYBOUND bound;
	bound.cElements = 2;
	bound.lLbound = 0;

	dim.parray = SafeArrayCreate(VT_INT, 1, &bound);

	void * pArrayData = NULL;
	SafeArrayAccessData(dim.parray, &pArrayData);
	((int *)pArrayData)[0] = 32;
	((int *)pArrayData)[1] = ip.labels.size();
	SafeArrayUnaccessData(dim.parray);

	p->AddParameter("LABELS", "Marker Labels", "POINT", 0, -1, 2, dim, NULL);
	Sleep(500);


	int labl = p->GetParameterIndex("POINT", "LABELS");
	p->AddParameterData(labl, ip.labels.size());
	int desc = p->GetParameterIndex("POINT", "DESCRIPTIONS");

	
	for(int i = 0; i < ip.labels.size(); i++){

		label.SetString(ip.labels[i].c_str());
		p->SetParameterValue(labl, i, label);
		p->SetParameterValue(desc, i, label);
	}



	p->SaveFile(bStrFile, 1);
	p->Close();
	}
	catch (const _com_error& Err)
	{

		TCHAR buff[255] = { 0 };
		std::cout << Err.Description() << " " << Err.ErrorMessage() << " " << Err.ErrorInfo();
		Sleep(10000);


	}

	}
	CoUninitialize();
	return 0;
}
