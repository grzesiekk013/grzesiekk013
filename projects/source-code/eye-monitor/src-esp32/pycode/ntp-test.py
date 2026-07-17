import socket
import struct
import time

def get_ntp_time(host="194.146.251.100"):
    port = 123
    buffer = 48
    msg = b'\x1b' + 47 * b'\0'
    
    with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as s:
        s.settimeout(5)
        s.sendto(msg, (host, port))
        msg, _ = s.recvfrom(buffer)
    
    t = struct.unpack("!12I", msg)[10]
    t -= 2208988800  # Convert NTP time to Unix timestamp
    
    return time.ctime(t)

if __name__ == "__main__":
    print("NTP Time:", get_ntp_time())
