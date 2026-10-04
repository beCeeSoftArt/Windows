#include "GENERICA_NDOW.h"
#include <conio.h>

void main (int argc, char *argv [], char *envp [])
{
 NET_WWW_DOWNLOADER *ndow = new NET_WWW_DOWNLOADER(GetStdHandle (STD_OUTPUT_HANDLE)); 
 char destination_file[MAX_PATH];
 char source_file[MAX_PATH];
 if (argc > 2)
   {
    lstrcpyn (source_file,   argv [1], INTERNET_MAX_URL_LENGTH);
    lstrcpyn (destination_file, argv [2], MAX_PATH);
    if (source_file[0])
      {
       ndow -> copydown(source_file, destination_file);;
      } 
   }
 else
   {
    printf("\n\nGENERICA Console Network File Downloader\n\n");
    printf("\nNetwork Download Via HTTP & FTP & GOPHER\n");
    printf("\nUsage:");
    printf("\n       NDOW <Source Url> <Destination File>\n");
    printf("\nNDOW Version 1.0.0.2 ©Copyright Andre' Spitzner, 2001");
    printf("\n\n");
    getch();
   }  
}


