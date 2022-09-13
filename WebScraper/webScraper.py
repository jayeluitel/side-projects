import requests
from bs4 import BeautifulSoup
import pandas as pd

jobTitles = []
employers = []
jobLocations = []
url = "https://realpython.github.io/fake-jobs/"
page = requests.get(url)
if page.status_code == 200:
        soup = BeautifulSoup(page.content, "html.parser")
        results = soup.find(id="ResultsContainer")
        jobs = results.find_all("div", class_="card-content")
        index = 0
        while index < len(jobs) - 1:
            job = jobs[index]
            jobTitle = job.find(class_="title is-5").get_text().strip()
            employer = job.find(class_="subtitle is-6 company").get_text().strip()
            jobLocation = job.find(class_="location").get_text().strip()
            jobTitles.append(jobTitle)
            employers.append(employer)
            jobLocations.append(jobLocation)
            index += 1
df = pd.DataFrame({
    "Job Title": jobTitles,
    "Employer": employers,
    "Job Location": jobLocations
    })
df.to_csv("pythonJobs.csv", index=False)
print("pythonJobs.csv saved. Program completed.")
            
