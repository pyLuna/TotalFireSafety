const wrapper = document.querySelector(".wrapper");
const fileName = document.querySelector(".image-file-name label");
const defaultBtn = document.querySelector("#default-btn");
const customBtn = document.querySelector("#custom-btn");
const cancelBtn = document.querySelector("#img-cancel-btn span");
const img = document.querySelector(".user-pic");
let regExp = /[0-9a-zA-Z\^\&\'\@\{\}\[\]\,\$\=\!\-\#\(\)\.\%\+\~\_ ]+$/;
function defaultBtnActive() {
    defaultBtn.click();
}
defaultBtn.addEventListener("change", function () {
    const file = this.files[0];
    if (file) {
        const reader = new FileReader();
        reader.onload = function () {
            const result = reader.result;
            img.src = result;
            wrapper.classList.add("img-active");
        }
        cancelBtn.addEventListener("click", function () {
            img.src = "";
            wrapper.classList.remove("img-active");
        })
        reader.readAsDataURL(file);
    }
    if (this.value) {
        let valueStore = this.value.match(regExp);
        fileName.textContent = valueStore;
    }
});

let infopopup = document.getElementById("info-popup");

function infoOpenPopup() {
    infopopup.classList.add("info-popup-show")
}

function infoClosePopup() {
    infopopup.classList.remove("info-popup-show")
}

let popup = document.getElementById("popup");

function openPopup() {
    popup.classList.add("open-popup-show")
}

function closePopup() {
    popup.classList.remove("open-popup-show")
}

let canpopup = document.getElementById("can-popup");

function canOpenPopup() {
    canpopup.classList.add("can-popup-show")
}

function canClosePopup() {
    canpopup.classList.remove("can-popup-show")
}

let rempopup = document.getElementById("rem-popup");

function remOpenPopup() {
    rempopup.classList.add("rem-popup-show")
}

function remClosePopup() {
    rempopup.classList.remove("rem-popup-show")
}