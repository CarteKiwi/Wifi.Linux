# Wifi.Linux


This library allows you to connect WIFI on Linux (wrap commands like wpa_cli of wpa_supplicant)
	- Retrieve available WIFIs for dotnet app hosted on Linux.
	- Connect to selected WIFI



SCENARIO

You need to connect WIFI to you server app using a client app, connected via Bluetooth.
- Server app running on a Raspberry Pi (Debian) (with or without UI)
- Client app running on mobile / device
- Both use Bluetooth (see my other repository "Bluetooth.Linux" to advertise BLE from Linux)
- Once connected, client ask host for Wifis list
- Host return Wifis
- Client select Wifi and send info to host
- Host initiate connection (ask for password, connect and reboot)




STEPS for SCENARIO

(Bluetooth.Linux) - Initialize BLE      

(Bluetooth.Linux) - (Linux) Advertise BLE

(Bluetooth.Linux) - (client) Scan BLE    

(Bluetooth.Linux) - (client) Connect BLE

(Bluetooth.Linux) - (client) Connected - Send data to ignite action (ask for Wifis)

(Bluetooth.Linux) - (Linux) Connected - Received data, telling what to do

(Wifi.Linux) - (Linux) Retrieve Wifis list and send it back to client  

(Wifi.Linux) - (client) Choose a ESSID                                  

(Wifi.Linux) - (Linux) Receive choosen ESSID                           

(Wifi.Linux) - (Linux) Connect to selected WIFI                        

