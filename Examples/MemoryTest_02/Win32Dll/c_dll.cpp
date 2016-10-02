#include "stdafx.h"
#include <stdio.h> 
#include <iostream>

extern "C"
{
	/// <summary>
	/// Gets the data.
	/// </summary>
	/// <param name="sourceArray">The source array.</param>
	/// <param name="size">The size.</param>
	/// <returns></returns>
	__declspec(dllexport) int GetData(double* sourceArray, int size)
	{
		auto result = 0;
		try
		{
			if (sourceArray == nullptr)
				return -99;

			double *vla = static_cast<double*>(malloc(size * sizeof(double)));


			for (auto i = 0; i < size; i++)
				vla[i] = i * 0.0002;

			memcpy(sourceArray, vla, size * sizeof(double));

			free(vla);

			result = size;
		}
		catch (std::exception& oe)
		{
			printf(oe.what());

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Gets the data array.
	/// </summary>
	/// <param name="sourceArray">The source array.</param>
	/// <param name="arraySize">Size of the array.</param>
	/// <param name="size">The size.</param>
	/// <returns></returns>
	__declspec(dllexport) int GetDataArray(double** sourceArray, int arraySize, int size)
	{
		auto result = 0;
		try
		{
			if (sourceArray == nullptr)
				return -99;

			printf("Size %i, Lenght: %i ", arraySize, size);

			for (auto n = 0; n < arraySize; n++)
			{
				auto vla = static_cast<double*>(malloc(size * sizeof(double)));

				for (auto i = 0; i < size; i++)
					vla[i] = n + i * 0.0001;

				memcpy(sourceArray[n], vla, size * sizeof(double));
				free(vla);
			}
			result = size;
		}
		catch (std::exception& oe)
		{
			printf(oe.what());

			result = -1;
		}

		return result;
	}

}

