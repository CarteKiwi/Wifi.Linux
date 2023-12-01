# Wifi.Linux

Work in progress....




AIM

Retrieve available WIFIs for dotnet app hosted on Linux.




SCENARIO

You need to connect WIFI to you server app using a client app, connected via Bluetooth.
- Server app running on a Raspberry Pi (Debian) (with or without UI)
- Client app running on mobile / device
- Both use Bluetooth (see my other repository "Bluetooth.Linux" to advertise BLE from Linux)





STEPS

(Bluetooth.Linux) - Initialize BLE      

(Bluetooth.Linux) - (Linux) Advertise BLE

(Bluetooth.Linux) - (other) Scan BLE    

(Bluetooth.Linux) - (other) Connect BLE

(Bluetooth.Linux) - (other) Connected - Send data to ignite action (ask for Wifis)

(Bluetooth.Linux) - (Linux) Connected - Received data, telling what to do

(Wifi.Linux) - (Linux) Retrieve Wifis list and send it back to client  

(Wifi.Linux (WIP)) - (other) Choose a ESSID                                  

(Wifi.Linux (WIP)) - (Linux) Receive choosen ESSID                           

(Wifi.Linux (WIP)) - (Linux) Connect to selected WIFI                        

