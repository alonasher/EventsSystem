# Event Tracking System

A full-stack event tracking solution that captures user interactions via a client-side application, processes them asynchronously using a message broker, and stores them in a time-series database for analysis.

## 🚀 Architecture

The system is built using a **Microservices Architecture** approach:

graph LR
    Client[React Client] -->|Web Worker| API[Backend API (.NET 9)]
    API -->|Produce| Kafka[Kafka Broker]
    Kafka -->|Consume| Worker[Worker Service (.NET 9)]
    Worker -->|Write| InfluxDB[InfluxDB]

* **Frontend:** React (Vite + TypeScript). Implements **Web Workers** for off-main-thread event submission.
* **Backend API:** ASP.NET Core 9 Web API (Producer).
* **Message Broker:** Apache Kafka (running in KRaft mode).
* **Consumer:** .NET Background Service (Worker).
* **Database:** InfluxDB (Time-series database optimized for event logs).
* **Infrastructure:** Docker Compose (for Kafka, InfluxDB, and UI tools).

---

## 🛠️ Prerequisites

* **Docker Desktop** (running)
* **.NET 9.0 SDK**
* **Node.js** (v18 or higher)

---

## 🏃‍♂️ Setup & Run Instructions

### 1. Infrastructure (Docker)

Start the necessary infrastructure services (Kafka, InfluxDB, Kafka UI):

```bash
docker-compose up -d

```

*Wait a few seconds for the containers to fully initialize.*

### 2. Backend API (Producer)

Open a terminal in the `src/EventsApi` folder:

```bash
cd src/EventsApi
dotnet run

```

*The API will start on port http://localhost:5150.*

### 3. Worker Service (Consumer)

Open a new terminal in the `src/EventsConsumer` folder:

```bash
cd src/EventsConsumer
dotnet run

```

*You should see logs indicating connection to Kafka.*

### 4. Frontend (React)

Open a new terminal in the `src/events-ui` folder:

```bash
cd src/events-ui
npm install
npm run dev

```

*Open your browser at http://localhost:5173*

---

## 📖 Usage

The application consists of two main pages:

### 1. Recording Page (Home)

* Simulate user events using the provided controls:
* **Button Click**
* **Text Input** (triggers on blur)
* **Checkbox Toggle**


* **Note:** Events are sent via a **Web Worker** to ensure the main UI thread remains unblocked.

### 2. Analyze Page

* View the history of recorded events.
* **Filter:** Use the "From" and "To" date pickers to filter events by time range.
* Data is fetched from InfluxDB via the API.

---

## 💡 Key Design Decisions & Assumptions

* **Web Workers:** implemented to meet the bonus requirement and demonstrate performance optimization by handling network requests in a background thread.
* **InfluxDB:** Chosen over a relational DB (SQL) because event logs are time-series data by nature. InfluxDB offers superior write performance and time-based query capabilities.
* **Kafka:** Used to decouple the high-throughput ingestion (API) from the processing layer (Consumer), ensuring the API remains responsive even under heavy load.
* **Clean Architecture:** The frontend is separated into Views, Components, and Services to maintain code modularity.

---

## 📞 Contact

Submitted by: Alon Asher
Repository: [Link to your repo]

```
