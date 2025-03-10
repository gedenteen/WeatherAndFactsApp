# 'WeatherAndFacts' Application

![Movie_002](https://github.com/user-attachments/assets/f27028cd-dba3-4492-a165-d69853c086ec)

This is a pet project that displays UI data retrieved from a backend using API requests. The app consists of two tabs: **Weather Forecast** and **Facts List**, with data updates managed through a queued request system. This project demonstrates efficient API interaction, request management, and UI state handling in Unity. Technologies used:
* **Zenject** – Dependency Injection
* **UniRx + UniTask** – Reactive programming & async handling
* **DoTween** – Animations

## Features
### Weather Forecast
* Fetches weather updates every 5 seconds when the user is on the tab.
* Cancels or removes pending requests when switching tabs.
* Displays a weather icon and temperature.

### Facts List
* Loads 10 facts when opened, showing a loading indicator.
* Facts are clickable and open a popup with details.
* If another fact is clicked, the previous request is canceled.
* Switching tabs cancels the current request.

### Request Queue System
* All API calls are queued and executed sequentially.
* Active requests cancel when switching tabs or changing selections.
