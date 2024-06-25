import time
from signalrcore.hub_connection_builder import HubConnectionBuilder
import logging 


# Configuration
hub_url = "http://localhost:8080/presenceHub"
token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI4IiwidW5pcXVlX25hbWUiOiJKZWFuIERlYnJhcyIsInJvbGUiOiJQcm9mZXNzZXVyIiwiU2Nob29sTmFtZSI6IkVTR0kiLCJTY2hvb2xJZCI6IjEiLCJuYmYiOjE3MTkzNDMyNzQsImV4cCI6MTcxOTk0ODA3NCwiaWF0IjoxNzE5MzQzMjc0fQ.3OBpHNJZfGtPfr13zfoE7YjE6DIhyX-HDD8nwrihcT8"  # Remplacez par votre token JWT
subject_hour_id = 78  # ID de l'heure de cours
student_id = 1  # ID de l'étudiant

#logging.basicConfig(level=logging.DEBUG)
#logger = logging.getLogger(__name__)


# Validate the token format
if token.count('.') != 2:
    raise ValueError("The provided JWT token is not well-formed. Please check the token.")

# SignalR connection
hub_connection = HubConnectionBuilder()\
    .with_url(hub_url, options={"access_token_factory": lambda: token})\
    .build()

print('passage ici')


def on_receive_code(code):
    print(code)

hub_connection.on("ReceiveCode", on_receive_code)

# Function to connect and join the room
def connect_and_join():
    try:
        hub_connection.start()
        #logger.info("Connected to SignalR hub.")
        hub_connection.send("JoinRoom", [subject_hour_id])
    except Exception as e:
        #logger.error(f"Error starting connection or joining room: {e}")
        hub_connection.stop()
        time.sleep(5)
        connect_and_join()

# Initial connection
connect_and_join()

# Keep the connection open
try:
    while True:
        time.sleep(15)
except KeyboardInterrupt:
    #logger.info("Stopping connection.")
    hub_connection.stop()


