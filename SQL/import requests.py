import requests
import json
from datetime import datetime, timedelta

# Définir les paramètres constants
url = 'http://localhost:8080/api/SubjectsHour'
headers = {
    'accept': 'text/plain',
    'Authorization': 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxMCIsInVuaXF1ZV9uYW1lIjoiSmVhbi1FdWQgRGVicmFzIiwicm9sZSI6IkVsZXZlIiwiU2Nob29sTmFtZSI6IkVTR0kiLCJTY2hvb2xJZCI6IjEiLCJuYmYiOjE3MTkwNDY4NDIsImV4cCI6MTcxOTY1MTY0MiwiaWF0IjoxNzE5MDQ2ODQyfQ.tVvQd6jp8Qzq1nxNbc1KKMbri00uGsGz_iTBj-jiuwM',
    'Content-Type': 'application/json'
}

# Définir les autres paramètres nécessaires pour la requête
subject_id = 1
building_id = 3
room = "A02"
start_date = datetime(2024, 6, 23)
end_date = datetime(2024, 6, 30)

# Fonction pour générer les données de la requête
def generate_request_data(date_start, date_end):
    return {
        "subjectsHour_Subjects_Id": subject_id,
        "subjectsHour_Building_Id": building_id,
        "subjectsHour_Room": room,
        "subjectsHour_DateStart": date_start.isoformat(),
        "subjectsHour_DateEnd": date_end.isoformat()
    }

# Envoyer les requêtes
for day in range((end_date - start_date).days + 1):
    current_day = start_date + timedelta(days=day)
    for hour in range(6):  # 6 cours par jour
        start_time = current_day + timedelta(hours=hour * 1.5)
        end_time = start_time + timedelta(hours=1.5)
        
        data = generate_request_data(start_time, end_time)
        response = requests.post(url, headers=headers, data=json.dumps(data))
        
        if response.status_code == 200:
            print(f"Successfully created course on {start_time}")
        else:
            print(f"Failed to create course on {start_time}: {response.status_code} - {response.text}")

print("All requests have been sent.")
