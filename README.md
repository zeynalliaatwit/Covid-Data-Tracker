# COVID Data Tracker

## Overview

**COVID Data Tracker** is a desktop application designed to visualize COVID-19 trends using wastewater-based surveillance data. The project focuses on presenting case trends over time in a clean, minimal UI while highlighting important events such as mask mandates and variant emergence.

This project demonstrates data visualization, UI design, and data handling using **C# and WPF**.

---

## Background

### Wastewater-Based COVID-19 Surveillance

Instead of relying only on clinical testing, this project uses wastewater data to track COVID-19 spread earlier and more effectively.

* **North System (Green):** Cambridge, Somerville, Boston, and surrounding areas
* **South System (Orange):** Brookline, Newton, Quincy, and surrounding areas
* Both systems send wastewater to the **Deer Island Treatment Plant**, where samples are tested for **SARS‑CoV‑2 RNA levels**.

This approach helps detect outbreaks earlier than traditional testing methods.

---

## Features

* 📈 Visualizes daily COVID-19 case data over time
* 📅 Highlights key dates on graphs (e.g., mask mandates, variant emergence)
* 🎯 Focus on clean, minimal, and user-friendly UI
* 🔍 Supports basic data filtering

---

## Technology Stack

* **Language:** C#
* **Framework:** WPF (.NET)
* **Charting Library:** LiveCharts
* **Data Source:** CSV files

---

## Data Handling & Challenges

* Parsing COVID-19 case data from CSV files
* Handling missing or incomplete data
* Designing responsive UI using XAML
* Binding data efficiently using WPF data binding

---

## Variant Insight: Omicron

The project also highlights real-world epidemic behavior observed during the Omicron variant:

* Rapid spread
* Shorter incubation period
* Natural and faster decline compared to earlier variants

---

## What I Learned

* WPF data binding and MVVM-friendly design
* Interactive charting with LiveCharts
* Data visualization best practices
* Handling real-world datasets with inconsistencies

---

## How to Run

1. Clone the repository
2. Open the solution in **Visual Studio**
3. Run the application
4. Click load button and select the data.csv file from Data folder

---

## Future Improvements

* Add more advanced filtering options
* Support multiple data sources
* Improve UI animations and interactivity
* Add export functionality for charts

---

## Author

**Amil Zeynalli**
April 2025

---

## License

This project is for educational purposes.
