#include "GENERICA_NDOW.h"
 #include <windows.h>    // Windows Main Include file
 #include <stdio.h>      // Standard I/O routines
 #include <stdarg.h>     // Defines ANSI-style macros 
 #include <string.h>     // String manipulation functions
 #include <dir.h>        
 #include <dirent.h>     
 #include <wininet.h>
//------------------------------------------------------------------------------ 
// Generic Constructor
//------------------------------------------------------------------------------ 
NET_WWW_DOWNLOADER::NET_WWW_DOWNLOADER(HANDLE Hstdout)
{
 try
    {
     hstdout = Hstdout;
     source_file_url       = &source_file_url_[0];                
     destination_file_path = &destination_file_path_[0];
     strcpy(source_file_url, "");
     strcpy(destination_file_path, "");
    }
 catch ( ... ) {}  
}
//------------------------------------------------------------------------------ 
// Generic Destructor
//------------------------------------------------------------------------------ 
NET_WWW_DOWNLOADER::~NET_WWW_DOWNLOADER()
{
 try
    {
    }
 catch ( ... ) {}  
}
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
long int __fastcall NET_WWW_DOWNLOADER::copydown(char * source_file, 
                                                                       char * destination_file)
{
 long int ret = 0;
 try
    {
     strcpy(source_file_url, source_file);
     strcpy(destination_file_path, destination_file);
     HINTERNET session;

     printf ("Opening Network Session.\n");
     session = InternetOpen("GENERICA",
                              LOCAL_INTERNET_ACCESS,
                              NULL, 
                              INTERNET_INVALID_PORT_NUMBER,
                              INTERNET_FLAG_DONT_CACHE);
     if (session != NULL)
       {
        ret = get_file (session, source_file, destination_file);
        printf ("Closing Network Session.\n");
        InternetCloseHandle (session);
       }
     else
       {
        printf ("\nWARNING: Network Session Can Not Opened!\n");
       } 
    }
 catch ( ... ) {ret = -1;}  
}
//------------------------------------------------------------------------------ 
// Get WWW File
//------------------------------------------------------------------------------ 
long int __fastcall NET_WWW_DOWNLOADER::get_file(HINTERNET hsession,
                                                 char      source [],
                                                 char      sdestination [])
{
 long int ret = 0;
 try
    {
     HINTERNET hsource;
     HANDLE    destination;

     printf ("\nOpening \"%s\"\n", source);
     hsource = InternetOpenUrl(hsession,source,NULL,0xFFFFFFFF,INTERNET_FLAG_DONT_CACHE | INTERNET_FLAG_RAW_DATA,0);
     if (hsource != NULL)
       {
        if (!destination_file_path[0])
          {
           printf ("Output directed to STDOUT\n");
           destination = hstdout;
          }
        else
          {
           printf ("Creating \"%s\"\n", sdestination);
           destination = CreateFile (sdestination,
                                     GENERIC_WRITE,
                                     FILE_SHARE_READ,
                                     NULL,
                                     CREATE_NEW,
                                     FILE_FLAG_WRITE_THROUGH | 
                                     FILE_FLAG_SEQUENTIAL_SCAN,
                                     NULL);
          }
        if (destination != INVALID_HANDLE_VALUE)
          {
           printf ("\nStarting Network Download\n");
           netdownload_file(hsource, destination);

           if (destination != hstdout)
             {
              printf ("Closing \"%s\"\n", sdestination);
              CloseHandle (destination);
             }
          }
        else
          {
           printf ("\nWARNING: Downloaded Output File Can Not Created!\n\n");
           printf ("Closing \"%s\"\n", source);
           InternetCloseHandle (hsource);
          }
       }    
     else
       {
        printf ("\nWARNING: Network Adress Can Not Opened!\n\n");
       } 
    }
 catch ( ... ) {ret = -1;}  
}
//------------------------------------------------------------------------------ 
// Download File INTERNET INTERFACE
//------------------------------------------------------------------------------ 
long int __fastcall NET_WWW_DOWNLOADER::netdownload_file(HINTERNET hsource,
                                                         HANDLE    hdestination)
{
 long int ret = 0;
 try
    {
     UINT       uretry;
     BOOL       ifok;
     BYTE       file_buffer [FILE_BUFFER_SIZE];
     SYSTEMTIME start_time;

     if (hdestination == hstdout)
     printf ("\n                [ BEGIN NETWORK DONWLOAD]                \n");
     writefilelength = 0;
     GetSystemTime (&start_time);
     do
       {
        if (hdestination != hstdout)
          {
           printf ("\r%lu Bytes Transferred ... ", writefilelength);
           printf ("(%lu Bytes/sec)         ", BytesPerSecond (&start_time, writefilelength));
          }
        uretry = 0;
        do
          {
           readfilecount = 0;
           ifok = InternetReadFile (hsource,file_buffer,FILE_BUFFER_SIZE,&readfilecount);
           }
        while (!(readfilecount || ifok || uretry++ == RETRY_READ));
    
        if (readfilecount)
          {
           writefilecount = 0;
           ifok = WriteFile (hdestination,file_buffer,readfilecount,&writefilecount,NULL);
           if (ifok = ifok && (readfilecount == writefilecount))
             {
              writefilelength += writefilecount;
             } 
           else
             {
              readfilecount = 0;
              printf ("\nWARNING: Error Can Not Write Data!");
             }
          }
        else
          {
           if (!ifok)
             {
              printf ("\nWARNING: Error Read Data!");
             } 
          }
       }
     while (readfilecount);

     if (hdestination == hstdout)
       {
         printf ("\n                 [ END NETWORK TRANSFER ]\n");
       }
     printf ("\n\n");
    }
 catch ( ... ) {ret = -1;}  
}
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
DWORD __fastcall NET_WWW_DOWNLOADER::BytesPerSecond (SYSTEMTIME *start_time,
                                                     DWORD       bytes)
{
 DWORD ret;
 try
    {
     SYSTEMTIME current_time;
     DWORD      start_second;
     DWORD      seconds;

     start_second = (((DWORD) start_time->wSecond)          ) +
                    (((DWORD) start_time->wMinute) * 60     ) +
                    (((DWORD) start_time->wHour  ) * 60 * 60);

     GetSystemTime (&current_time);
  
     seconds = (((DWORD) current_time.wSecond)          ) +
               (((DWORD) current_time.wMinute) * 60     ) +
               (((DWORD) current_time.wHour  ) * 60 * 60) -
               start_second;

     if (seconds & 0x80000000) 
       {
        seconds += 24 * 60 * 60;
       }
     
     ret = (seconds ? (bytes / seconds) : 0);
    }
 catch ( ... ) {ret = -1;}  
}
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
/*
long int __declspec(dllexport) __fastcall NET_WWW_DOWNLOADER::
{
 long int ret = 0;
 try
    {

    }
 catch ( ... ) {ret = -1;}  
}
*/ 
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
/*
long int __declspec(dllexport) __fastcall NET_WWW_DOWNLOADER::
{
 long int ret = 0;
 try
    {

    }
 catch ( ... ) {ret = -1;}  
}
*/ 
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
/*
long int __declspec(dllexport) __fastcall NET_WWW_DOWNLOADER::
{
 long int ret = 0;
 try
    {

    }
 catch ( ... ) {ret = -1;}  
}
*/ 
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
/*
long int __declspec(dllexport) __fastcall NET_WWW_DOWNLOADER::
{
 long int ret = 0;
 try
    {

    }
 catch ( ... ) {ret = -1;}  
}
*/ 
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
/*
long int __declspec(dllexport) __fastcall NET_WWW_DOWNLOADER::
{
 long int ret = 0;
 try
    {

    }
 catch ( ... ) {ret = -1;}  
}
*/ 
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
/*
long int __declspec(dllexport) __fastcall NET_WWW_DOWNLOADER::
{
 long int ret = 0;
 try
    {

    }
 catch ( ... ) {ret = -1;}  
}
*/ 
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
/*
long int __declspec(dllexport) __fastcall NET_WWW_DOWNLOADER::
{
 long int ret = 0;
 try
    {

    }
 catch ( ... ) {ret = -1;}  
}
*/ 
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
/*
long int __declspec(dllexport) __fastcall NET_WWW_DOWNLOADER::
{
 long int ret = 0;
 try
    {

    }
 catch ( ... ) {ret = -1;}  
}
*/ 
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
/*
long int __declspec(dllexport) __fastcall NET_WWW_DOWNLOADER::
{
 long int ret = 0;
 try
    {

    }
 catch ( ... ) {ret = -1;}  
}
*/ 
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
/*
long int __declspec(dllexport) __fastcall NET_WWW_DOWNLOADER::
{
 long int ret = 0;
 try
    {

    }
 catch ( ... ) {ret = -1;}  
}
*/ 
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
/*
long int __declspec(dllexport) __fastcall NET_WWW_DOWNLOADER::
{
 long int ret = 0;
 try
    {

    }
 catch ( ... ) {ret = -1;}  
}
*/ 
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
/*
long int __declspec(dllexport) __fastcall NET_WWW_DOWNLOADER::
{
 long int ret = 0;
 try
    {

    }
 catch ( ... ) {ret = -1;}  
}
*/ 
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
/*
long int __declspec(dllexport) __fastcall NET_WWW_DOWNLOADER::
{
 long int ret = 0;
 try
    {

    }
 catch ( ... ) {ret = -1;}  
}
*/ 
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
/*
long int __declspec(dllexport) __fastcall NET_WWW_DOWNLOADER::
{
 long int ret = 0;
 try
    {

    }
 catch ( ... ) {ret = -1;}  
}
*/ 
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
/*
long int __declspec(dllexport) __fastcall NET_WWW_DOWNLOADER::
{
 long int ret = 0;
 try
    {

    }
 catch ( ... ) {ret = -1;}  
}
*/ 
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
/*
long int __declspec(dllexport) __fastcall NET_WWW_DOWNLOADER::
{
 long int ret = 0;
 try
    {

    }
 catch ( ... ) {ret = -1;}  
}
*/ 
//------------------------------------------------------------------------------ 
// Download
//------------------------------------------------------------------------------ 
/*
long int __declspec(dllexport) __fastcall NET_WWW_DOWNLOADER::
{
 long int ret = 0;
 try
    {

    }
 catch ( ... ) {ret = -1;}  
}
*/ 
 