# Getting Started with the SDK

1. **Clone this repository**  
	```powershell
	git clone https://github.com/Orpius/SDK.git
	````

2. **Open the solution** in **Visual Studio 2022** (or later).

3. **Build and run** the sample application to confirm everything is working locally.
   By default, the sample project runs on:

	```
	https://localhost:7194
	```

---

# Using Operations

Operations allow your own application to communicate with your Orpius AI Agents.

# Creating your own Custom Tools

To allow Orpius to call your tools, Orpius needs to know the **publicly reachable URL** of your server.
Since development machines usually run on `localhost`, we recommend creating a secure tunnel.

[Learn how to create a secure channel](../CreatingAChannel)

