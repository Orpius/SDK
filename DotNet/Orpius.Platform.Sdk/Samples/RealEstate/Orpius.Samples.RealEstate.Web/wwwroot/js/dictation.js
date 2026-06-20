(function () {
	const SpeechRecognition =
		window.SpeechRecognition || window.webkitSpeechRecognition;

	const buttons = document.querySelectorAll("[data-dictation-target]");

	for (const button of buttons) {
		const targetSelector = button.getAttribute("data-dictation-target");
		const target = document.querySelector(targetSelector);

		if (!SpeechRecognition || !target) {
			button.disabled = true;
			button.title = "Speech recognition is not available in this browser.";
			continue;
		}

		const recognition = new SpeechRecognition();

		recognition.continuous = false;
		recognition.interimResults = true;
		recognition.lang = button.getAttribute("data-dictation-language") || "en-GB";

		let originalText = "";

		button.addEventListener("click", function () {
			originalText = target.value;

			button.classList.add("recording");
			button.textContent = "Listening...";

			recognition.start();
		});

		recognition.onresult = function (event) {
			let finalText = "";
			let interimText = "";

			for (let index = event.resultIndex; index < event.results.length; index++) {
				const transcript = event.results[index][0].transcript;

				if (event.results[index].isFinal) {
					finalText += transcript;
				}
				else {
					interimText += transcript;
				}
			}

			const separator =
				originalText.trim().length === 0
					? ""
					: " ";

			target.value = originalText + separator + finalText + interimText;
			target.dispatchEvent(new Event("input", { bubbles: true }));
		};

		recognition.onerror = function () {
			button.classList.remove("recording");
			button.textContent = "🎙";
		};

		recognition.onend = function () {
			button.classList.remove("recording");
			button.textContent = "🎙";
		};
	}
})();