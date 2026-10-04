/*----------------------------------------------------------------------------*/
/*--Programm-Datei:---CALL_VVCO.H---------------------------------------------*/
/*--Programm-Teil-:---Via-Voice-Speech-Engine-Connector-CONTROL-Modul---------*/
/*--Programmierer-:---AndrÚ-Spitzner------------------------------------------*/
/*--Copyright-----:---AndrÚ-Spitzner------------------------------------------*/
/*--License-------:---Free-For-Non-Commercial-Or-Developper-Use-Only!---------*/
/*--Support-------:---AndrÚ-Spitzner-E-Mail-bcare@T-Online.de-----------------*/
/*----------------------------------------------------------------------------*/
#ifndef __GENERICA_NDOW_H__
 #define __GENERICA_NDOW_H__
//------------------------------------------------------------------------------
// Includes
//------------------------------------------------------------------------------
 #include <windows.h>    // Windows Main Include file
 #include <stdio.h>      // Standard I/O routines
 #include <stdarg.h>     // Defines ANSI-style macros 
 #include <string.h>     // String manipulation functions
 #include <dir.h>        
 #include <dirent.h>     
 #include <wininet.h>
//------------------------------------------------------------------------------
#define FILE_BUFFER_SIZE 4096
#define RETRY_READ         10
//------------------------------------------------------------------------------ 
// Download Http Ftp Gopher File To Disk
//------------------------------------------------------------------------------ 
 class  NET_WWW_DOWNLOADER
      {
       public  : // Generic Constructor
                 NET_WWW_DOWNLOADER(HANDLE Hstdout);
                 // Generic Destructor
                 ~NET_WWW_DOWNLOADER();

                 // Download A Url File To Local Logical Drive
                 long int  __fastcall copydown(char * source_file, char *destination_file);
                 
       private : char   source_file_url_[INTERNET_MAX_URL_LENGTH];
                 char   destination_file_path_[MAX_PATH];
                 char * source_file_url;
                 char * destination_file_path;
                 HANDLE hstdout;
                 DWORD  readfilecount;
                 DWORD  writefilecount;
                 DWORD  writefilelength;

                 // Get The Count Of processed Bytes Per Minute
                 DWORD __fastcall BytesPerSecond (SYSTEMTIME *start_time,DWORD bytes);
                 // Download The File From Network
                 long int __fastcall netdownload_file(HINTERNET hsource,HANDLE hdestination);
                 // Get The File
                 long int __fastcall get_file(HINTERNET hsession,char source[],char sdestination[]);

                 
      };
//------------------------------------------------------------------------------
#endif
//------------------------------------------------------------------------------

