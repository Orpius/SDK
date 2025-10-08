# Orpius User Guide

# Welcome to Orpius

This guide provides an overview of Orpius and instructions for getting started.

# Introducing Orpius

Orpius is a system that gives developers the infrastructure they need to connect AI capabilities to their applications with minimal setup.

You can use Orpius as a:
* **Foundation for integrating AI into existing applications**
Provides all the infrastructure you need to add AI features without the overhead of building it yourself. 
* **Foundation for developing new AI applications, systems, and tools**
Gives you a ready environment for building AI-powered solutions from the ground up.
* **Productivity tool**
All configuration is handled through the Orpius Console, a desktop client that runs on the same infrastructure available to your applications so anything the platform can do, the Console can do directly. The Console is also aware of the system itself, letting you set up parts such as events through an interactive chat interface.

## Orpius Provides
Orpius provides a complete foundation for building and running AI-driven applications.

It includes:
* **Integration with external systems** via API-driven events or customer-facing AI agents
* **Built-in capabalities**
    * **Code execution tooling** currently supports C#, with Java and VB coming soon
    * **Events registeration and triggering**
    * **Notification and messaging** for collaboration and **team awareness** across time zones
    * **Scheduling** for running tasks at defined times or intervals
    * **Memory**
    * **Video feed image analysis**
    * **Web page retrieval**
* **Custom Agent creation**
* **Multi-model support** 
* **Shared storage**
* **Permissions and Security**
* **Secrets system**
* **Orchestration** 
* **Tools Integration** with Built-in tools that provide core capabilities out of the box and support for custom tooling




## Built-in Productivity Features
The following examples show some of the productivity features available directly in the Console. These may give you a sense of what you can achieve in your own applications, though with custom tooling you can expand the capabilities much further. 

For example, you can use chat in the Console to instruct Orpius to:
* Schedule and carry out tasks
* Read, analyze, and write files
* Apply image analysis to video streams in real time
* Send emails and notifications to members of your team
* Organize meeting that suit everyone’s time zone

# Deployment
Currently, the Orpius platform is deployed as a single-tenant cloud service. Support for on-premises deployments is planned.

# System Structure Overview
Orpius is organized around organizations and spaces. 

At the top level is the **Organization**, which represents your company or team. Each Organization contains one or more **Spaces**.   

When Orpius is first installed, a default organization and space are created automatically for each user. 

## Roles and Structure

**System Owner** – The person responsible for deploying and maintaining the Orpius installation within a stack. The owner manages the underlying environment, including settings such as mail server configuration, member access restrictions, and inference or application limits. Each installation runs in its own private cloud environment.

**User** - Anyone with access to Orpius in the same stack, without system-level privileges.

**Organization** – Represents a company or team within the system. Each organization has its own infrastructure, storage, and members. A user can belong to multiple organizations.

**Space** – A workspace within an organization, similar to a project or repository. Spaces contain all the resources for a specific project, such as agents, tools, models, operations, and events. Each organization can have multiple spaces, and access is controlled through roles and permissions.

# Getting started
Before you can start working with Orpius, you will need:

* **Orpius Client Application (Orpius Console)** – A lightweight desktop client for configuring everything from models to agents and tools.
* **SDK for the Orpius platform** – Includes libraries that simplify integration into your applications, plus a sample application that shows you exactly how to get started.
* **Access to Email server (System owners only)** – Because Orpius runs in your own private cloud, email handling is not provided as a shared service. An email server is required during initial setup so new users can verify their email addresses (a sending-only server is sufficient). It will also be used to send notifications or messages to team members. In addition, we have future plans to introduce email driven events.
* **Access to your own LLM provider** – Orpius connects to AI models you choose whether hosted on-premises or in the cloud. It currently supports OpenAI and Azure OpenAI, with more providers to follow.

In a nutshell, how it works:

1. **Configure** – Define your models, agents, events, and operations in the Orpius Console.
(Operations are the chat entry points (AI assistants) through which your systems or users interact with your agents.)
2. **Integrate with your system** – Add the SDK libraries to your application, and by coping and pasting the credentials from Orpius client to your application, embed the AI assistant directly into your application.
3. **Execute** – The AI assistant can now call your server-side tools. The Orpius Server runs them and manages orchestration, integration, state, storage, and security.

# Installing the Orpius Console Application

The Orpius Console is a lightweight desktop client that connects directly to the Orpius infrastructure.

To get started, you’ll receive a message from us containing:
1. A link to download the **Orpius Console**.
2. The Orpius Server URL.
3. Your Access Key (System owners only).

Follow these steps:
1. Open the download link and click Download Orpius Console.
2. Run the installer after the file finishes downloading.
3. Once installation is complete, launch the Orpius Console.
4. Enter the Server URL into the field provided.
5. Enter the Access Key (System owners only).

# Setting up Orpius for the first time
## Setting up email server (System owners only)
During first-time setup, you’ll be prompted to enter your email server details. The email server is used by Orpius to handle email requests such as sending messages to team members. Setting up the email server as part of the initial setup is a required step because it allows new users to verify their email address. 

Enter your email server details.
You can update this information later in **System Settings**.

## Creating an user account
Every Orpius user creates and signs into a user account.

During the first time setup, you’ll be prompted to create your account.

* Enter your account **Username**, **Email**, **Given Name**, **Surname** and **Password** into the fields provided.

**Note:** At present, account information can’t be changed after your account is created. Options to update these details will be available in an upcoming version.

You’ll receive a confirmation email from the system address you configured during email server setup (e.g. noreply@yourdomain.com) containing a code.

If you see an error message, click **Previous** and check that your email address and email server information are correct.

# First Launch
After the initial setup is completed, Orpius opens in the Spaces view. Each user begins with an auto-generated default Space. For more details on creating and managing Spaces, see Spaces.

# Spaces
A space is your working area inside the Orpius Console. Each space keeps everything related to a project or team in one place. You can also invite members (inside or outside your organization) to collaborate in a space. Members can be invited into a space and granted access through roles.

Your Spaces are listed on the Space View page, where you can:
* **Open** an existing space
* **Create** a new space
* **Delete** a space

Within a Space, you can:
* View notifications
* View and manage scheduled tasks
* Upload and download files to and from your isolated storage
* Invite and manage members for collaboration
* Configure your Models, Agents, Tools, Operations, Secrets, and Events
* Interact with LLMs via chat 

## Creating a Space
To create a new Space:
1.	From the **Space View** page, select **Create a Space**.
2.	Enter a name for the Space. 
3.	Confirm to create.

# Models
The system is designed to work with any AI model, whether hosted **on-premises** or in the **cloud**. Currently, OpenAI and Azure OpenAI are supported, with more providers to follow.

You can configure as many models as you need. For example, some agents might use GPT-5 for complex tasks, while others can run on a lower-cost model such as OpenAI gpt-5-mini. 

## Configuring a New Model

1. Open the **Models** view from the sidebar 
2. Select **'+'** button in the toolbar of the **Models** view
3. Enter the required information
4. Save

# Agents
In Orpius, Agents are consider users, just like humans. Orpius includes two built-in agents: **Orpius** and **Phaedra**.
* **Orpius** is the agent you interact with directly through chat.
* **Phaedra** is the agent that orchestrates non-interactive conversations, such as scheduled tasks and background activity execution.

Both Orpius and Phaedra are **organization-wide** agents available in every space. You can also configure **custom agents** dedicated to specific operations within a space.

By default, Orpius and Phaedra use the first LLM you define, but you can assign them to any of your available LLMs.

Agents in Orpius do not have fixed tools or workflows of their own. Instead, they draw from a shared pool of tools.

When integrating your application with Orpius, you configure a **Custom Agent**. This agent will handle all incoming requests from your application.

## Configuring a Custom Agent


1. Open the **Agents** view from the sidebar 
2. Select **'+'** button in the toolbar of the **Agents** view
3. Give the Agent a **Name**
4. Select a suitable **Model** for your Agent
5. Define the Agent’s **Persona**
6. Set the Agent’s **Temperature**
7. Provide further **Instructions** that will assist the agent in carrying out its duties.
8. Save.

# Agent Tools

Tools in Orpius are not tied to a single agent. They live in a shared pool that any agent can access if it has permission. Tools can be combined, triggered by events, and reused across different agents. Agents can autonomously select and combine the tools they need to complete a task.

## Build-in Tools

Orpius includes the following set of built-in tools:

* **Scheduler** – Allows an AI agent to add work items to be done at a future time. 
    * Schedules can be of the type:
        * **Daily** - to run at a specified time
        * **Weekly** - to run on a specified day of the week and specified time
        * **Monthly** – to run on a specified day of the month and time
        * **Yearly** - to run on a specified day of the month and time
        * **Interval** - to run after a specified period and may be set to repeat forever or loop for a specified number of times.
    * **WebSearch** – Allows an AI agent to search in internet using Bing. An API key is required. Note that Bing Search APIs retired on August 11, 2025.
    * **WebPageRetriever** – Retrieve the HTTP response for a specified URL. Allows for the use of web APIs.
    * **Notifier** – send notifications to a user. A notification may also be sent by email to the user. The user’s email is never provided to an agent.
    * **CodeExecution** – compiles and runs managed code and passes the results back to the agent. This allows the assistant to complete tasks that require calculations and storing and retrieving data from files. The execution environment is sandboxed and hosted within WebAssembly (Wasm), isolating it from other processes or code execution.
    * **ImageAnalysis** – Allows an agent to analyze an image, either from a specified image URL or by snapping a frame from a real-time video stream. 
    * **VideoFeedMonitor** – Real Time Streaming Protocol (RTSP). Downloads a still image from a video feed which can be used for image analysis.
    * **EventRegistry** – registers an event name that can be used by a remote API to trigger a task. The identifier can then be used with a web hook in the system.
    * **Memory** – Allows an agent to add a record of an event or experience to the agents memory store, enabling it to track changes across multiple tasks.
    * **EventTrigger** - triggers an event that was previously registered

The following **example** shows how a simple instruction can lead the agent to **autonomously** combine **multiple tools** to complete the task.

**Prompt:**

>Analyze <%=Key:WebCam%> every 2 minutes, record the findings into a file and notify John if cars are parked in the restricted space. 

**Note:** that the prompt may be made more descriptive, i.e. specifying the file name and extension, providing instructions on how to append the file and what information to record and how.)

What the agent does:
1. Retrieves the video stream URL **(Secrets)**
2. Connects to the video stream **(VideoFeedMonitor)**
3. Captures a frame every two minutes **(Scheduler)**
4. Analyses the image to detect parked cars **(ImageAnalysis)**
5. Writes the findings to a file **(CodeExecution)**
6. Notifies the user if an issue is detected **(Notifier)**


# Custom Tools

In addition to the built-in tools, you can register your own custom tools (plugins) with Orpius. These extend the system with domain-specific capabilities that your agents can use during inferencing.

## How it works:

1. **Register a custom tool** – Open the **Agent Tools** view from the sidebar and select **Custom Tools** .
2. **Integrate with your application** – Copy the **External ID** and **Access Key**, then paste them into **your application**.
3.**Expose functionality to Orpius** – Decorate your code with the [Tool] and [ToolMethod] attributes.
4. **Execute** – The Orpius SDK automatically receives and dispatches tool requests at runtime.

See the **SDK documentation** for more details on custom tools. Example implementations are included in the downloadable SDK.

# Events

Events provide a secure way to connect your systems with Orpius and perform activities in response to external triggers.

## How it Works:

* In the Console, you register events that external systems can call through a remote API to trigger tasks. Each event has a unique identifier that can be used in a webhook or integrated system. 
* After registering an event, you add the activities or follow-up events that should run when it is triggered. 
* Orpius then orchestrates the response in real time.

**Example of how events may flow in a Surveillance Application**
* A camera sends a motion detected event. 
* You configure this event to trigger a notification activity and an analysis activity.
* Orpius then orchestrates the response: it notifies the right user, launches the analysis, and, depending on the outcome, can automatically trigger follow-up actions such as raising an alert, starting a recording, or handing off to another tool.

(Note: In the current release, events are handled by the in-built Orpius agents.)

## Security and Control

Orpius is designed to be adaptive rather than follow rigid workflows. To keep this behaviour reliable, you can set **guardrails** by defining which **tools** and **constraints** an agent is allowed to use.

The event system can also provide a built-in **security layer** based on the principle of Segregation of Duties (SoD). Customer-facing agents can trigger events to request actions from more privileged agents, without having direct access to sensitive capabilities such as scheduling, storage, or team information.

## HTTP Parameters

* Parameters starting with an underscore ( _ ) are passed to your tool only.
* Parameters without an underscore are passed to your tool and the AI Agent.


## Setting up and event

There are two ways to define an event in Orpius: manually or through chat.

**Tip**: When defining activities that are triggered by an event, avoid using words such as schedule or references to the event itself in the task instruction. This can confuse the agent, leading it to interpret the instruction as a command to schedule itself or create a new event. Instead, phrase the instruction as an immediate action.


**Manual setup**
1. **Register the Event**
    * Open the **Events** view from the sidebar.
    * Select the **'+'** button in the toolbar of the **Events** view
    * In the **Event Configuration** view, enter the event name (for example, *NewOrderReceived*).
2. **Define the Triggered Work (Activity)**
    * In the **Event Triggered Work** view, select the **'+'** button in the toolbar of the **Event Triggered Work** view.
    * Provide a **Title** and clear **Instructions** for the agent (for example, *"send a notification to the user"*).
    * You can define multiple activities for the same event.
    * *(Upcoming feature)* Supply Custom Tools that the agent can use when executing the task.
3. **Trigger and Execute**
    * When an external system fires the *NewOrderReceived* event, Orpius automatically executes the defined activity.



**Setup via Chat**

Orpius understands events and can be instructed through chat to create new ones with specific actions.

* Start a new **Chat** 
* For example, you could say: *"Create an event named 'NewOrderReceived' that sends me a notification with the order details when triggered."*
* Orpius will then register the event and set up the instructions accordingly, including your user ID for proper execution.
* Open the **Events** view to review or modify the newly created event.


# Isolated Storage

Each organisation has an allocated shared storage area that can be used across its spaces.

Files uploaded here can be accessed directly by agents within the same space. This allows you to provide data, documents, or other resources for your agents to use while keeping everything securely contained.

Agents can read, write, and modify files within this storage.

For example, you could upload a sales report and ask Orpius:

*"Summarise the key trends from last quarter's sales and write the summary to a file called SalesSummary.txt. If the file already exists, append to it."*

Access to files within a space is controlled by permission levels. Members with roles above Participant have access to files in that space.

## Uploading files

* Open the **Files** view from the sidebar
* Click the **Upload** button in the toolbar of the **Files** view and select the files you want to upload
* Click **Open**


## Downloading files

* Open the **Files** view from the sidebar
* Select the file(s) you wish to download and click the **Download** butoon in the toolbar of the **Files** view
* Choose the folder where you want the files saved

**Please note**: in the pre-release version, files downloaded locally are not being managed. This means they may overwrite an existing file in the chosen folder without warning.

## Creating files using chat

Orpius can create a wide variety of file types depending on your needs. Common examples include text files (e.g. .txt, .csv), code files (e.g. .cs, .py, .js), document files (e.g. .md, .html), and data files (e.g. .json, .xml).
* Start a new **Chat**
* For example, you could say: *"Create a file called test.txt and insert 10 city names."*
* Orpius will create a new file named test.txt containing the 10 city names and save it in your isolated storage directory.

# Team Management

Each organization can contain multiple spaces, and each space can have as many members as needed. 

Orpius has built-in team awareness across time zones and can plan or take actions based on that.

## How it works:

* For example, you can use the chat and ask *"Who is on my team?"*
* Orpius will respond with a list of team members. 
* Or you might ask *"Please thank John for the lovely flowers"*
* Orpius will send John both an email and a notification on your behalf.

In system settings can (System Owners only:
• Set the maximum number of users in an organisational unit.
• Set the maximum number of administrators in an organisational unit.
• Restrict new users to having an email address within the organisation by configuring a regular expression. For example: ^[A-Za-z0-9._%+-]+@yourcompany\.(com|org|net)$

Adding Members
The Owner and Administrator of a space can invite members, either from within or outside the organisation. Each member is assigned a role that determines their permissions:
• Participant – minimal permissions. Can receive notifications (email) and perform tasks.
• Editor – includes all Participant privileges, plus the ability to modify content and manage tasks in the space.
• Administrator – can manage members, configure settings, and oversee activity.
• Owner – full control over the space, including deleting it or transferring ownership.
