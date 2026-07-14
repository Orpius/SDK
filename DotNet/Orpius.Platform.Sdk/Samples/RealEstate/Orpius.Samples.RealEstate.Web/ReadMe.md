## Setup

# Configure the Orpius Agent and Operation

Before running the sample, create an Agent and Operation in the Orpius Console. The Agent defines how the LLM should behave, while the Operation provides the communication layer between this application and Orpius.

## 1. Create the Agent

1. Create a new Agent in the Orpius Console.
2. Assign the Agent to a language model.
3. Set the Agent **Persona** to:

```text
You are a helpful assistant for handling real estate operations for estate agents.
You use tools to help estate agents create property listings and match potential buyers,
known as applicants, to property listings.
```

## 2. Create the Operation

1. Create a new Operation.
2. Give it a name, such as **RealEstate**.
3. Assign the Agent you created above to the Operation.
4. Set the Operation **Instructions** to:

```text
Handle real estate agent requests to create property listings,
add applicants, match applicants to property listings, 
and notify estate agents when suitable matches are found.
```

## 3. Update the `appsettings.Development.json` File

Update the `appsettings.Development.json` file in this project with the Operation credentials from the Orpius Console.

1. In the Operation view, copy the Operation **External Id** and **Access Key 1**.

2. Set the tool registration values using the **External ID** and **Access Key** found under **Agent Tools / Custom Tools**.

3. Set the `IncomingUrl` value to the URL provided by your Cloudflare secure channel.

For local development, Orpius needs a secure channel back to this sample application so it can call the registered tools.

See the [SDK User Guide](https://github.com/Orpius/SDK/blob/main/docs/UserGuide/CreatingAChannel/index.md) for more information on creating a secure channel to Orpius for local development.
