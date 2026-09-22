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

## Key Features

### 1. Authentication & Security

* **JWT Access & Refresh Tokens:** Secure logins with short-lived tokens and revocable refresh tokens so users stay logged in safely.


* **Role-Based Access (RBAC):** Restricts admin features while ensuring customers can only view their own information and bookings.


* **Rate Limiting:** Protects login endpoints from brute-force attempts and spam.



### 2. Smart Booking Rules

* **No Double Bookings:** Blocks conflicting reservations for the same space during the same time window.


* **No Unpaid Bookings:** Prevents users with pending debts or unpaid invoices from making new reservations.


* **Grace Periods & Late Fees:** Automatically bills customers if they stay beyond the allowed grace period.



### 3. Fast Data Queries

* **Server-Side Pagination:** Delivers data in small, organized pages (`PagedResponse`) so large tables load fast without crashing client apps.


* **Flexible Filtering:** Search reservations and users by name, date range, or status straight from the URL.


* **Optimized Database Queries:** Uses direct SQL execution (`ExecuteUpdateAsync`, `ExecuteDeleteAsync`) and read-only queries to keep memory use low.



---

## Tech Stack

* **Language:** C#
* **Framework:** ASP.NET Core Web API
* **Database & ORM:** SQL Server with Entity Framework Core


* **Security:** JWT (JSON Web Tokens), Password Hashing, ASP.NET Core Rate Limiting


* **Architecture:** 3-Tier Layered Architecture (Controllers, Services, Repositories)