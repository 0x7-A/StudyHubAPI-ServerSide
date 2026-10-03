# StudyHubAPI

StudyHubAPI is a RESTful backend API built with **ASP.NET Core** and **Entity Framework Core** to manage study workspaces, bookings, and customer billing. It provides a clean, secure system for reservations, check-ins, automated fees, and user accounts.

---

## What the System Does

* **Workspace Management:** Keeps track of study desks, private meeting rooms, hourly rates, capacities, and maintenance status.

* **Reservation Engine:** Prevents double-bookings by checking time conflicts before confirming any reservation.

* **Check-In & Check-Out:** Manages guest arrivals, departures, cancellations, and automatically calculates extra charges if someone stays past their booked time.

* **Billing & Payments:** Tracks payments for bookings, late fees, and overdue balances.

* **Reviews & Ratings:** Lets customers rate their completed visits and calculates average scores for each workspace.

* **User Accounts:** Manages Customers, Admins, and SuperAdmins with proper role permissions.

---

## 📂 Project Structure

```text
StudyHubAPI/
├── Controllers/              # API Endpoints & Request Handling
├── Data/                     # DbContext & Database Configurations
├── Models/                   # Data Models & Transfer Objects
│   ├── DTOs/                 # Data Transfer Objects grouped by feature
│   ├── Entities/             # Database Entities
│   ├── Enums/                # Business & Status Enumerations
│   └── Filter/               # Dynamic Query & Search Filters
├── Properties/               # Launch & Environment Settings
├── Repositories/             # Data Access Layer & DB Queries
├── Services/                 # Business Logic Layer
└── Utils/                    # Helper Classes & Utilities