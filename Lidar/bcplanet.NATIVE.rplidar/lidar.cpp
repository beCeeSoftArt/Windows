// ***********************************************************************
// Assembly         : bcplanet.NATIVE.rplidar.dll
// Author           : André Spitzner
// Created          : 03-04-2021
//
// Last Modified By :  André Spitzner
// Last Modified On : 03-05-2021
// ***********************************************************************
// <copyright file="lidar.cpp" company="beCee Soft Art">
//     Copyright (c) André Spitzner. All rights reserved.
//    
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met :
//
// 1. Redistributions of source code must retain the above copyright notice,
// this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
// this list of conditions and the following disclaimer in the documentation
// and /or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
// PURPOSE ARE DISCLAIMED.IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
// EXEMPLARY, OR CONSEQUENTIAL DAMAGES(INCLUDING, BUT NOT LIMITED TO,
//PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
// OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT(INCLUDING NEGLIGENCE OR
// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// </copyright>
// <summary></summary>
// ***********************************************************************
#include "pch.h"
#include "lidar.h"

#include <cstdio>
#include <iostream>


#include "../RPLIDAR_SDK/sdk/sdk/include/rplidar.h"

extern "C"
{
	/// <summary>
	/// Initialize driver.
	/// </summary>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarInitializeDriver(void)
	{
		auto result = 0;
		try
		{
			/*printf("CreateDriver: Struct Size=%llu\n", sizeof(rplidar_response_measurement_node_hq_t));
			printf("CreateDriver: Array Size=%llu", 8192 * sizeof(rplidar_response_measurement_node_hq_t));

			return  result;*/
			if (lidar_driver == nullptr)
			{
				lidar_driver = rp::standalone::rplidar::RPlidarDriver::CreateDriver(rp::standalone::rplidar::DRIVER_TYPE_SERIALPORT);
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Dispose driver.
	/// </summary>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarDisposeDriver(void)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr)
			{
				if (lidar_driver->isConnected())
				{
					result = lidar_driver->stop();

					if (result != 0)
						return result;

					result = lidar_driver->stopMotor();

					if (result != 0)
						return result;

					lidar_driver->disconnect();
				}

				rp::standalone::rplidar::RPlidarDriver::DisposeDriver(lidar_driver);
				lidar_driver = nullptr;
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Connect to the lidar.
	/// </summary>
	/// <param name="comPort">The COM port.</param>
	/// <param name="baudRate">The baud rate.</param>
	/// <param name="flag">The flag.</param>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarConnect(const char* comPort, uint32_t baudRate = 115200, uint32_t flag = 0)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& !lidar_driver->isConnected())
			{
				result = lidar_driver->connect(comPort, baudRate, flag);
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Disconnect from lidar.
	/// </summary>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarDisconnect(void)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& !lidar_driver->isConnected())
			{
				lidar_driver->disconnect();
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Check if connected to lidar.
	/// </summary>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarIsConnected(void)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				result = 1;
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Reset lidar device.
	/// </summary>
	/// <param name="timeOut">The time out.</param>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarReset(uint32_t timeOut = 2000)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& !lidar_driver->isConnected())
			{
				result = lidar_driver->reset(timeOut);
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Clear serial cache.
	/// </summary>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarClearSerialCache(void)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				result = lidar_driver->clearNetSerialRxCache();
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Get lidar health status.
	/// </summary>
	/// <param name="health">The health.</param>
	/// <param name="timeout">The timeout.</param>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarGetHealth(rplidar_response_device_health_t& health, uint32_t timeout = 2000)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				result = lidar_driver->getHealth(health, timeout);
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Get lidar device information.
	/// </summary>
	/// <param name="info">The information.</param>
	/// <param name="timeout">The timeout.</param>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarGetDeviceInfo(rplidar_response_device_info_t& info, uint32_t timeout = 2000)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				result = lidar_driver->getDeviceInfo(info, timeout);
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Set motor PWM.
	/// </summary>
	/// <param name="pwm">The PWM.</param>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarSetMotorPWM(uint16_t pwm)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				result = lidar_driver->setMotorPWM(pwm);
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Set spin speed.
	/// </summary>
	/// <param name="rpm">The RPM.</param>
	/// <param name="timeout">The timeout.</param>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarSetSpinSpeed(uint16_t rpm, uint32_t timeout = 2000)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				result = lidar_driver->setLidarSpinSpeed(rpm, timeout);
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Start motor.
	/// </summary>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarStartMotor(void)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				result = lidar_driver->startMotor();
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Stop motor.
	/// </summary>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarStopMotor(void)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				result = lidar_driver->stopMotor();
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Check if motor support control.
	/// </summary>
	/// <param name="support">The support.</param>
	/// <param name="timeout">The timeout.</param>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarCheckIfMotorSupportControl(bool& support, uint32_t timeout = 2000)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				result = lidar_driver->checkMotorCtrlSupport(support, timeout);
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Check if is Tof device.
	/// </summary>
	/// <param name="isTofLidar">The is tof lidar.</param>
	/// <param name="timeout">The timeout.</param>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarCheckIfIsTofDevice(bool& isTofLidar, uint32_t timeout = 2000)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				result = lidar_driver->checkIfTofLidar(isTofLidar, timeout);
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Get frequency.
	/// </summary>
	/// <param name="scanMode">The scan mode.</param>
	/// <param name="count">The count.</param>
	/// <param name="frequency">The frequency.</param>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarGetFrequency(const rp::standalone::rplidar::RplidarScanMode& scanMode, uint64_t count, float& frequency)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				const size_t count_size = count;
				result = lidar_driver->getFrequency(scanMode, count_size, frequency);
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Start normal scan.
	/// </summary>
	/// <param name="force">The force.</param>
	/// <param name="timeout">The timeout.</param>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarStartNormalScan(bool force, uint32_t timeout = 2000)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				result = lidar_driver->startScanNormal(force, timeout);
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	__declspec(dllexport) int LidarStartScan(bool force, uint16_t mode, rp::standalone::rplidar::RplidarScanMode* scanMode = nullptr, uint32_t options = 0)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				std::vector<rp::standalone::rplidar::RplidarScanMode> modeVec;

				lidar_driver->getAllSupportedScanModes(modeVec);

				auto modeIter = modeVec.begin();
				for (; modeIter != modeVec.end(); ++modeIter)
				{
					printf(
						"Mode: %s\nDistance: %f\nsScam Mode %i\nId: %i\nSample time:  %fµs\n\n", 
						modeIter->scan_mode,
						static_cast<double>(modeIter->max_distance),
						modeIter->ans_type,
						modeIter->id,
						static_cast<double>(modeIter->us_per_sample));
				}
				
				//result = lidar_driver->startScan(force, typicalScan, options, scanMode);
				result = lidar_driver->startScanExpress(force, mode, options, scanMode);
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Stop the device.
	/// </summary>
	/// <param name="timeout">The timeout.</param>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarStop(uint32_t timeout = 2000)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				result = lidar_driver->stop(timeout);
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}



	///// <summary>
	///// Grab scan data.
	///// </summary>
	///// <param name="nodeBuffer">The node buffer.</param>
	///// <param name="count">The count.</param>
	///// <param name="timeout">The timeout.</param>
	///// <returns>int.</returns>
	//__declspec(dllexport) int LidarGrabScanDataHq(rplidar_response_measurement_node_hq_t* nodeBuffer, uint64_t count, uint32_t timeout = 2000)
	//{
	//	auto result = 0;
	//	try
	//	{
	//		/*if (lidar_driver != nullptr
	//			&& lidar_driver->isConnected())
	//		{
	//			size_t count_size = count;
	//			result = lidar_driver->grabScanDataHq(nodeBuffer, count_size, timeout);

	//			count = count_size;
	//		}*/
	//	}
	//	catch (std::exception& oe)
	//	{
	//		printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

	//		result = -1;
	//	}

	//	return result;
	//}

	/// <summary>
	/// Sort scan data ascend.
	/// </summary>
	/// <param name="nodeBuffer">The node buffer.</param>
	/// <param name="count">The count.</param>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarSortScanDataAscend(rplidar_response_measurement_node_hq_t* nodeBuffer, uint64_t count)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				printf("LidarSortScanDataAscend: Struct Size=%llu\r\n", sizeof(rplidar_response_measurement_node_hq_t));
				printf("LidarSortScanDataAscend: Array Size=%llu\r\n", count);

				size_t count_size = count;
				result = lidar_driver->ascendScanData(nodeBuffer, count_size);
				
				printf("LidarSortScanDataAscend: Result: %i\r\n\r\n", result);
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}


	/// <summary>
	/// Get scan data with interval.
	/// </summary>
	/// <param name="nodeBuffer">The node buffer.</param>
	/// <param name="count">The count.</param>
	/// <returns>int.</returns>
	__declspec(dllexport) int LidarGetScanDataWithIntervalHq(rplidar_response_measurement_node_hq_t* nodeBuffer, uint64_t count)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				size_t count_size = count;
				result = lidar_driver->getScanDataWithIntervalHq(nodeBuffer, count_size);
			}
		}
		catch (std::exception& oe)
		{
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	/// <summary>
	/// Sort scan data ascend.
	/// </summary>
	/// <param name="nodeBuffer">The node buffer.</param>
	/// <param name="count">The count.</param>
		/// <param name="count">The result count.</param>
	/// <param name="timeout">The timeout.</param>
	/// <returns>int.</returns>
	__declspec(dllexport) int _stdcall LidarGrabScanDataHq(rplidar_response_measurement_node_hq_t* nodeBuffer, uint64_t count, uint64_t* resultCount, uint32_t timeout = 2000)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				size_t count_size = count;

				printf("LidarGrabScanDataHq: Struct size: %llu\n", sizeof(rplidar_response_measurement_node_hq_t));
				printf("LidarGrabScanDataHq: Array input size: %llu\n", count_size);

				result = lidar_driver->grabScanDataHq(nodeBuffer, count_size, timeout);
				
				printf("LidarGrabScanDataHq::grabScanDataHq result: %i\n", result);

				*resultCount = static_cast<uint64_t>(count_size);


				//bool isDataReceived = false;
				if (IS_OK(result))
				{
					result = lidar_driver->ascendScanData(nodeBuffer, count_size);

					if (IS_OK(result))
					{
						//for (int index = 0; index < static_cast<int>(count_size); index++)
						//{							
						//		//isDataReceived = true;
						//		printf("%s theta: %03.2f Dist: %08.2f Q: %d \r\n",
						//			(nodeBuffer[index].flag & RPLIDAR_RESP_MEASUREMENT_SYNCBIT) ? "S " : "  ",
						//			static_cast<double>(nodeBuffer[index].angle_z_q14) * 90.0 / 16384.0,
						//			static_cast<double>(nodeBuffer[index].dist_mm_q2) / 4.0,
						//			nodeBuffer[index].quality);							
						//}
					}
				}
								
				printf("LidarGrabScanDataHq: Result count: %llu\r\n", *resultCount);
				printf("LidarGrabScanDataHq: Copied elements: %i\r\n", static_cast<int>(count_size));
				//printf("LidarGrabScanDataHq: Data: %s\r\n\r\n", isDataReceived ? "Data received" : "No data");

			}
		}
		catch (std::exception& oe)
		{
			printf("Error");
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	int CreateCheckSum(char* pNMEA)
	{
		int i;
		int iXOR;
		int c;
		// Calculate checksum ignoring any $'s in the string
		for (iXOR = 0, i = 0; i < strlen(pNMEA); i++)
		{
			c = static_cast<unsigned char>(pNMEA[i]);
			if (c == '*') break;
			if (c != '$') iXOR ^= c;
		}
		return iXOR;
	}

	/// <summary>
	/// Lidars the get nmea.
	/// </summary>
	/// <param name="sentences">The sentences.</param>
	/// <param name="inputSize">Size of the input.</param>
	/// <param name="resultSize">Size of the result.</param>
	/// <param name="sensorId">The sensor identifier.</param>
	/// <param name="timeout">The timeout.</param>
	/// <returns>int .</returns>
	__declspec(dllexport) int _stdcall LidarGetNmea(char* sentences[], uint64_t inputSize, uint64_t* resultSize, uint32_t sensorId, uint32_t timeout = 2000)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				rplidar_response_measurement_node_hq_t nodeBuffer[8192];
				rplidar_response_measurement_node_hq_t *nodes = &nodeBuffer[0];
				size_t count = _countof(nodeBuffer);

				printf("LidarGrabScanDataHq: Struct size: %llu\n", sizeof(rplidar_response_measurement_node_hq_t));
				printf("LidarGrabScanDataHq: Array input size: %llu\n", count);

				result = lidar_driver->grabScanDataHq(nodes, count, timeout);

				printf("LidarGrabScanDataHq::grabScanDataHq result: %i\n", result);

				*resultSize = static_cast<uint64_t>(count);


				if (IS_OK(result))
				{
					result = lidar_driver->ascendScanData(nodes, count);

					printf("LidarGrabScanDataHq::ascendScanData result: %i\n", result);

					if (IS_OK(result))
					{
						for (int index = 0; index < static_cast<int>(count); index++)
						{
							char buffer[MAXCHAR];
							
							sprintf_s(buffer, MAXCHAR, "$bclidar,%i,%s,%03.2f,%3.2f,%d*",
								sensorId,
								nodes[index].flag & RPLIDAR_RESP_MEASUREMENT_SYNCBIT ? "S" : "N",
								static_cast<double>(nodes[index].angle_z_q14) * 90.0 / 16384.0,
								static_cast<double>(nodes[index].dist_mm_q2) / 4.0,
								nodes[index].quality);

							sprintf_s(sentences[index], MAXCHAR, "%s%X", buffer, CreateCheckSum(buffer));
						}
					}
				}

				printf("LidarGrabScanDataHq: Result count: %llu\r\n", *resultSize);
				printf("LidarGrabScanDataHq: Copied elements: %i\r\n", static_cast<int>(count));

			}
		}
		catch (std::exception& oe)
		{
			printf("Error");
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}

	rplidar_response_measurement_node_hq_t nodeBuffer[8192];
	rplidar_response_measurement_node_hq_t* nodes = &nodeBuffer[0];

	__declspec(dllexport) int _stdcall LidarGetStringData(
		char* sentences[], 
		uint64_t inputSize, 
		uint64_t* resultSize, 
		uint32_t timeout = 2000)
	{
		auto result = 0;
		try
		{
			if (lidar_driver != nullptr
				&& lidar_driver->isConnected())
			{
				size_t count = _countof(nodeBuffer);

				/*printf("LidarGrabScanDataHq: Struct size: %llu\n", sizeof(rplidar_response_measurement_node_hq_t));
				printf("LidarGrabScanDataHq: Array input size: %llu\n", count);*/

				result = lidar_driver->grabScanDataHq(nodes, count, timeout);

				//printf("LidarGrabScanDataHq::grabScanDataHq result: %i\n", result);

				*resultSize = static_cast<uint64_t>(count);

				if (IS_OK(result))
				{
					result = lidar_driver->ascendScanData(nodes, count);

					//printf("LidarGrabScanDataHq::ascendScanData result: %i\n", result);

					if (IS_OK(result))
					{
						for (uint64_t index = 0; index < *resultSize; index++)
						{
							sprintf_s(
								sentences[index],
								MAXCHAR,
								"%03.2f;%3.2f",
								static_cast<double>(nodes[index].angle_z_q14) * 90.0 / 16384.0,
								static_cast<double>(nodes[index].dist_mm_q2) / 4.0);
						}
					}
				}

				printf("LidarGrabScanDataHq: Result count: %llu\r\n", *resultSize);
				printf("LidarGrabScanDataHq: Copied elements: %i\r\n", static_cast<int>(count));

			}
		}
		catch (std::exception& oe)
		{
			printf("Error");
			printf(oe.what());  // NOLINT(clang-diagnostic-format-security)

			result = -1;
		}

		return result;
	}	
}
