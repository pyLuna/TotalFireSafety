let category = document.getElementById("selcat");
let selclass = document.getElementById("selclass");
let seltype = document.getElementById("seltype");
let filtered = [];

let newLabels = [];
let newQuantity = [];

category.addEventListener("change", function () {
	FilterItems();
});
selclass.addEventListener("change", function () {
	FilterItems();
});
seltype.addEventListener("change", function () {
	FilterItems();
});
function GetAll() {
	fetch("/Admin/DBResult")
		.then(res => {
			if (res.ok) {
				// API request was successful
				return res.json();
			} else {
				console.log("error fetch");
			}
		})
		.then(data => {
			SetCard(data)
		})
		.catch(error => {
			window.location.replace('/Error/InternalServerError');
			console.error(error);
		});
}

function SetCard(array) {
	let users = document.getElementById('userLabel');
	let active = document.getElementById('actLabel');
	let supply = document.getElementById('supLabel');
	let actsupply = document.getElementById('actsupLabel');
	let deploy = document.getElementById('depLabel');
	let actdeploy = document.getElementById('actdepLabel');
	let purchase = document.getElementById('purLabel');
	let actpurchase = document.getElementById('actpurLabel');
	let allItems = document.getElementById('itemLabel');
	let crits = document.getElementById('critLabel');

	users.innerHTML= array.users;
	active.innerHTML= array.active_users;
	supply.innerHTML= array.supply;
	actsupply.innerHTML= array.rec_supply;
	deploy.innerHTML= array.deployment;
	actdeploy.innerHTML= array.rec_deployment;
	purchase.innerHTML= array.purchase;
	actpurchase.innerHTML = array.rec_purchase;
	allItems.innerHTML = array.items;
	crits.innerHTML = array.crit_items

}

//#region populate dropdown
function PopulateDropdown(array) {
	let type = [];
	let class1 = [];
	let categ = [];
	selclass.innerHTML = "";
	seltype.innerHTML = "";
	category.innerHTML = "";
	selclass.innerHTML += "<option selected disabled>Class</option>";
	seltype.innerHTML += "<option selected disabled>Type</option>";
	category.innerHTML += "<option selected disabled>Category</option>";
	for (let i = 0; i < array[0].length; i++) {
		categ.push(array[0][i].Category)
		for (let ii = 0; ii < array[0][i].Items.length; ii++) {
			type.push(array[0][i].Items[ii].Type);
			class1.push(array[0][i].Items[ii].Class);
		}
	}

	let newType = new Set(type);
	let newClass = new Set(class1);
	newType.forEach((item) => {
		if (item != "") {
			if (item != null) {
				seltype.innerHTML += `<option>${item}</option>`;
			}
		}
	});
	newClass.forEach((item) => {
		selclass.innerHTML += `<option>${item}</option>`;
	});
	categ.forEach((item) => {
		category.innerHTML += `<option>${item}</option>`;
	});
}
//#endregion

//#region flters
function FilterClass(value) {
	let arr = [];
	for (let i = 0; i < allItems[0].length; i++) {
		for (let ii = 0; ii < allItems[0][i].Items.length; ii++) {
			if (JSON.stringify(allItems[0][i].Items[ii].Class).toLowerCase().includes(value)) {
				arr.push(allItems[0][i].Items[ii]);
			}
		}
	}
	return arr;
}
function FilterCategory(value) {
	let arr = [];
	for (let i = 0; i < allItems[0].length; i++) {
		if (JSON.stringify(allItems[0][i].Category).toLowerCase().includes(value)) {
				for (let ii = 0; ii < allItems[0][i].Items.length; ii++) {
					arr.push(allItems[0][i].Items[ii]);
				}
			}
		}
	return arr;
}

function FilterType(type) {
	let arr = [];
	for (let i = 0; i < allItems[0].length; i++) {
		for (let ii = 0; ii < allItems[0][i].Items.length; ii++) {
				if (JSON.stringify(allItems[0][i].Items[ii].Type).toLowerCase().includes(type)) {
					arr.push(allItems[0][i].Items[ii]);
				}
		}
	}
	return arr;
}

function FilterAll(cat,type,cls) {
	let arr = [];
	for (let i = 0; i < allItems[0].length; i++) {
		if (JSON.stringify(allItems[0][i].Category).toLowerCase().includes(cat)) {
			for (let ii = 0; ii < allItems[0][i].Items.length; ii++) {
				if (JSON.stringify(allItems[0][i].Items[ii].Type).toLowerCase().includes(type) && JSON.stringify(allItems[0][i].Items[ii].Class).toLowerCase().includes(cls)) {
					arr.push(allItems[0][i].Items[ii]);
				}
			}
		}
	}
	return arr;
}

function FilterCat2D(cat,type,cls) {
	let arr = [];
	for (let i = 0; i < allItems[0].length; i++) {
		if (JSON.stringify(allItems[0][i].Category).toLowerCase().includes(cat)) {
			for (let ii = 0; ii < allItems[0][i].Items.length; ii++) {
				if (JSON.stringify(allItems[0][i].Items[ii].Type).toLowerCase().includes(type) || JSON.stringify(allItems[0][i].Items[ii].Class).toLowerCase().includes(cls)) {
					arr.push(allItems[0][i].Items[ii]);
				}
			}
		}
	}
	return arr;
}

function FilterType2D(type, cls) {
	let arr = [];
	for (let i = 0; i < allItems[0].length; i++) {
		for (let ii = 0; ii < allItems[0][i].Items.length; ii++) {
			if (JSON.stringify(allItems[0][i].Items[ii].Type).toLowerCase().includes(type) && JSON.stringify(allItems[0][i].Items[ii].Class).toLowerCase().includes(cls)) {
				arr.push(allItems[0][i].Items[ii]);
			}
		}
	}
	return arr;
}

function FilterItems() {
	let cat = category.options[category.selectedIndex]?.value;
	let type = seltype.options[seltype.selectedIndex]?.value;
	let cls = selclass.options[selclass.selectedIndex]?.value;

	if (cat != "Category" && type == "Type" && cls == "Class") {
		SendToChart(1);
	}
	else if (cat == "Category" && type != "Type" && cls == "Class") {
		SendToChart(2);
	}
	else if (cat == "Category" && type == "Type" && cls != "Class") {
		SendToChart(3);
	}
	else if (cat != "Category" && type != "Type" && cls != "Class") {
		SendToChart(4);
	}
	else if (cat != "Category" && type != "Type" || cls != "Class") {
		SendToChart(5);
	}
	else if (cat != "Category" || type != "Type" && cls != "Class") {
		SendToChart(6);
	}
}
// set chart 
function SendToChart(requestType) {
	let cat = category.options[category.selectedIndex]?.value;
	let type = seltype.options[seltype.selectedIndex]?.value;
	let cls = selclass.options[selclass.selectedIndex]?.value;
	let results = [];

	if (requestType == 1) {
		results = FilterCategory(cat.toLowerCase());
		let response = PopulateLabels(results, "item");
		SetChart(response.label, response.quant);
	}
	else if (requestType == 2) {
		results = FilterType(type.toLowerCase());
		let response = PopulateLabels(results, "item");
		SetChart(response.label, response.quant);
	}
	else if (requestType == 3){
		results = FilterClass(cls.toLowerCase());
		let response = PopulateLabels(results, "item");
		SetChart(response.label, response.quant);
	}
	else if (requestType == 4) {
		results = FilterAll(cat.toLowerCase(), type.toLowerCase(), cls.toLowerCase());
		let response = PopulateLabels(results, "item");
		SetChart(response.label, response.quant);
	}
	else if (requestType == 5) {
		results = FilterCat2D(cat.toLowerCase(), type.toLowerCase(), cls.toLowerCase());
		let response = PopulateLabels(results, "item");
		SetChart(response.label, response.quant);
	}
	else if (requestType == 6) {
		if (cat != "Category") {
			results = FilterCat2D(cat.toLowerCase(), type.toLowerCase(), cls.toLowerCase());
		}
		else {
			results = FilterType2D(cat.toLowerCase(), type.toLowerCase(), cls.toLowerCase());
		}
		let response = PopulateLabels(results, "item");
		SetChart(response.label, response.quant);
	}
}
//#endregion