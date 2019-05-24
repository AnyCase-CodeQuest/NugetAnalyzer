(function Accordion() {
	let acc = document.getElementsByClassName("div__accordion");
	let ii;

	for (ii = 0; ii < acc.length; ii++) {
		acc[ii].addEventListener("click", function () {
			this.classList.toggle("div__active");
			this.firstElementChild.classList.toggle("div__animation_active");
			var panel = this.nextElementSibling;
			if (panel.style.maxHeight) {
				panel.style.maxHeight = null;
			} else {
				panel.style.maxHeight = panel.scrollHeight + "px";
			}
		});
	}
})();