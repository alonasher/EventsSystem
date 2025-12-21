# Event Tracking System

A full-stack event tracking solution that captures user interactions via a client-side application, processes them asynchronously using a message broker, and stores them in a time-series database for analysis.

## 🚀 Architecture

The system is built using a **Microservices Architecture** approach and is fully containerized

* **Frontend:** React (Vite + TypeScript) served via Nginx. Implements **Web Workers** for off-main-thread event submission.
* **Backend API:** ASP.NET Core 9 Web API (Producer).
* **Message Broker:** Apache Kafka (running in KRaft mode).
* **Consumer:** .NET Background Service (Worker).
* **Database:** InfluxDB (Time-series database optimized for event logs).
* **Infrastructure:** Docker Compose (Orchestrates all services including UI and DB).

---

## 🛠️ Prerequisites

* **Docker Desktop** (Required)
* **Git**

*(Note: .NET SDK and Node.js are NOT required to run the application, as the build process happens inside Docker containers)*

---

## 🏃‍♂️ Setup & Run Instructions

### 1. Clone the Repository

```bash
git clone https://github.com/alonasher/EventsSystem.git
cd EventsSystem

```

### 2. Run the Environment

Build and start all services (Frontend, Backend, DB, Broker) with a single command:

```bash
docker-compose up -d --build

```

*Wait a minute for the containers to build and initialize.*

### 3. Access the Application

Once running, the services are available at:

* **🖥️ React Frontend:** http://localhost:5173
* **🔌 API Swagger:** http://localhost:5150/swagger
* **📊 Kafka UI:** http://localhost:8080
* **💾 InfluxDB:** http://localhost:8086

---

## 📖 Usage

The application consists of two main pages:

### 1. Recording Page (Home)

* Simulate user events using the provided controls:
* **Button Click**
* **Text Input** (triggers on change with debounce)
* **Checkbox Toggle**


* **Note:** Events are sent via a **Web Worker** to ensure the main UI thread remains unblocked.

### 2. Analyze Page

* View the history of recorded events.
* **Filter:** Use the "From" and "To" date pickers to filter events by time range.
* Data is fetched from InfluxDB via the API.

---

## 📝 Assumptions & Scope

* **Scalability over Complexity:** The system incorporates Apache Kafka and InfluxDB to demonstrate readiness for high-throughput scenarios, assuming that write performance and decoupling are critical for an event tracking system.
* **Authentication:** For the purpose of this assignment, user authentication and authorization were omitted to focus on the core event processing pipeline and ease of local deployment.
* **Data Consistency:** The solution implements an "At-Least-Once" delivery semantic via Kafka.
* **Network Reliability:** The client-side Web Worker assumes network connectivity. 

---

## 💡 Key Design Decisions

* **Web Workers:** Implemented to demonstrate performance optimization by handling network requests in a background thread.
* **InfluxDB:** Chosen over a relational DB (SQL) because event logs are time-series data by nature.
* **Kafka:** Used to decouple the high-throughput ingestion (API) from the processing layer (Consumer).
* **Dockerized Development:** All components, including the React UI build process, are defined in Dockerfiles to ensure a consistent environment across different machines.

---

## 📞 Contact

Submitted by: Alon Asher
