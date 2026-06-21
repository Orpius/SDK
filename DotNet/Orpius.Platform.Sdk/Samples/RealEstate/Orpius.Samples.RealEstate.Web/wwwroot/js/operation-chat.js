(function () {
	const forms = document.querySelectorAll("[data-operation-form]");

	for (const form of forms) {
		initializeOperationForm(form);
	}

	function initializeOperationForm(form) {
		const messagesTargetSelector = form.getAttribute("data-messages-target");
		const promptTargetSelector = form.getAttribute("data-prompt-target");
		const clearTargets = form.getAttribute("data-clear-targets");

		const messagesContainer = document.querySelector(messagesTargetSelector);
		const promptInput = document.querySelector(promptTargetSelector);
		const submitButton = form.querySelector("[type='submit']");

		if (!messagesContainer || !promptInput) {
			return;
		}

		form.addEventListener("submit", async function (event) {
			event.preventDefault();

			if (!promptInput.value.trim()) {
				return;
			}

			setBusy(submitButton, true);

			try {
				const formData = new FormData(form);

				const response = await fetch(form.action, {
					method: "POST",
					body: formData
				});

				if (!response.body) {
					appendMessage(messagesContainer, {
						role: "System",
						text: "The browser did not provide a readable response stream.",
						success: false
					});

					return;
				}

				await readMessageStream(response, messagesContainer);

				if (response.ok && clearTargets) {
					clearInputs(clearTargets);
				}
			}
			catch (error) {
				appendMessage(messagesContainer, {
					role: "System",
					text: "The request failed: " + error,
					success: false
				});
			}
			finally {
				setBusy(submitButton, false);
			}
		});
	}

	async function readMessageStream(response, messagesContainer) {
		const reader = response.body.getReader();
		const decoder = new TextDecoder();

		let buffer = "";

		while (true) {
			const result = await reader.read();

			if (result.done) {
				break;
			}

			buffer += decoder.decode(result.value, { stream: true });
			buffer = processBuffer(buffer, messagesContainer);
		}

		buffer += decoder.decode();

		if (buffer.trim().length > 0) {
			processLine(buffer.trim(), messagesContainer);
		}
	}

	function processBuffer(buffer, messagesContainer) {
		let newlineIndex = buffer.indexOf("\n");

		while (newlineIndex >= 0) {
			const line = buffer.substring(0, newlineIndex).trim();

			if (line.length > 0) {
				processLine(line, messagesContainer);
			}

			buffer = buffer.substring(newlineIndex + 1);
			newlineIndex = buffer.indexOf("\n");
		}

		return buffer;
	}

	function processLine(line, messagesContainer) {
		const message = JSON.parse(line);

		appendMessage(messagesContainer, message);
	}

	function appendMessage(messagesContainer, message) {
		const messageElement = document.createElement("div");
		messageElement.className = getMessageClass(message.role);

		const textElement = document.createElement("div");
		textElement.className = "message-text";
		textElement.textContent = message.text || "";

		messageElement.appendChild(textElement);

		if (normaliseRole(message.role) === "system") {
			const metaElement = createMetaElement(message);

			if (metaElement) {
				messageElement.appendChild(metaElement);
			}
		}

		messagesContainer.appendChild(messageElement);
		messagesContainer.scrollTop = messagesContainer.scrollHeight;
	}

	function createMetaElement(message) {
		if (!message.toolName && message.success === null && message.success === undefined) {
			return null;
		}

		const metaElement = document.createElement("div");
		metaElement.className = "message-meta";

		if (message.toolName) {
			const toolElement = document.createElement("span");
			toolElement.textContent = message.toolName;
			metaElement.appendChild(toolElement);
		}

		if (message.success !== null && message.success !== undefined) {
			const successElement = document.createElement("span");
			successElement.textContent = message.success ? "Succeeded" : "Failed";
			metaElement.appendChild(successElement);
		}

		return metaElement;
	}

	function getMessageClass(role) {
		const normalisedRole = normaliseRole(role);

		if (normalisedRole === "user") {
			return "operation-message user-message";
		}

		if (normalisedRole === "assistant") {
			return "operation-message assistant-message";
		}

		return "operation-message system-message";
	}

	function normaliseRole(role) {
		return String(role || "").toLowerCase();
	}

	function setBusy(button, busy) {
		if (!button) {
			return;
		}

		button.disabled = busy;
		button.textContent = busy ? "Working..." : "Send to Orpius";
	}

	function clearInputs(clearTargets) {
		const selectors = clearTargets
			.split(",")
			.map(selector => selector.trim())
			.filter(selector => selector.length > 0);

		for (const selector of selectors) {
			const element = document.querySelector(selector);

			if (element) {
				element.value = "";
			}
		}
	}
})();