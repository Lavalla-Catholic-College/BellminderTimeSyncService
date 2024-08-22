# BellminderTimeSyncService

A Windows service that synchronizes a Techenz Bellminder box with the computer's local time. This si a feature that should have been added from the beginning, but wasn't.

## Why?

We're still running a Bellminder system and due to its age (approximately 14 years old at this point), the time keeps slipping. This app syncs the time once a minute so it's always accurate.

## Building and installing

1. Clone this repo
2. Open in Visual Studio
3. Build and copy to it's final location (e.g. `c:\program files\BellminderTimeSync`)
4. Run this command from an elevated command line: `C:\Windows\Microsoft.NET\Framework64\<your installed .net version here>\installutil BellminderTimeSyncService.exe`

## Setup and running

1. Open the registry and navigate to: `Computer\HKEY_LOCAL_MACHINE\SOFTWARE\`
2. Create a new key called `BellminderTimeSync`
3. Create three `DWORD32` values in there, `COMPort`, `BaudRate` and `Interval` and set the values as required. `Interval` is in milliseconds
4. Open `services.msc` and locate the service. Start it.
5. Open `eventvwr.msc` and navigate to `Windows Logs > Applications` and look at the logs. If set up correctly, you should see success messages. A message will be written every time the time is synced. 
