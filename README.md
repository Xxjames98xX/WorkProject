# Project Setup Guide

This project contains:

- **Backend:** ASP.NET Web API, MS SQL, Loki, Grafana  
- **Frontend:** WPF Application

---

## ⛃ Backend

To run the backend using Docker, execute the following from the **root folder**:

```bash
docker compose up --build -d
```
*If running MS-SQL locally (host) instead of containerized, to change the connection strings. 
For more info refer to the DockerFile within the ASPWebAPI*

## 🖥️ Frontend
To run the front end, go to the folder inside WpfApp  
Then access the folder below.  It should contain the exe and dlls

```bash
publish - demo release 150925
```
Run **WpfApp.exe**
