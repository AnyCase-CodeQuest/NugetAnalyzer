(function accordion() {
    let accordionElement = $(".div__accordion");
	let i;

    for (i = 0; i < accordionElement.length; i++) {
		accordionElement[i].addEventListener("click", function () {
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