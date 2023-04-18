var category = document.getElementById("selcat");
var selclass = document.getElementById("selclass");
var seltype = document.getElementById("seltype");

var newLabels = [];
var newQuantity = [];

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
	var users = document.getElementById('userLabel');
	var active = document.getElementById('actLabel');
	var supply = document.getElementById('supLabel');
	var actsupply = document.getElementById('actsupLabel');
	var deploy = document.getElementById('depLabel');
	var actdeploy = document.getElementById('actdepLabel');
	var purchase = document.getElementById('purLabel');
	var actpurchase = document.getElementById('actpurLabel');
	var allItems = document.getElementById('itemLabel');
	var crits = document.getElementById('critLabel');

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
	var type = [];
	var class1 = [];
	var categ = [];
	for (var i = 0; i < array[0].length; i++) {
		categ.push(array[0][i].Category)
		for (var ii = 0; ii < array[0][i].Items.length; ii++) {
			type.push(array[0][i].Items[ii].Type);
			class1.push(array[0][i].Items[ii].Class);
		}
	}

	var newType = new Set(type);
	var newClass = new Set(class1);
	newType.forEach((item) => {
		if (item != null) {
			seltype.innerHTML += `<option>${item}</option>`;
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
	if (value == "all") {
		return allItems;
	}
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
	if (value == "all") {
		return allItems;
	}
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

function FilterItems() {
	var cat = category.options[category.selectedIndex]?.value;
	var type = seltype.options[seltype.selectedIndex]?.value;
	var cls = selclass.options[selclass.selectedIndex]?.value;

	if (category.selectedIndex != 0 && cat != "All" && seltype.selectedIndex == 0 && type == "All" && selclass.selectedIndex == 0 && cls == "All") {
		SendToChart(1);
	}
	else if (category.selectedIndex == 0 && cat == "All" && seltype.selectedIndex != 0 && type != "All" && selclass.selectedIndex == 0 && cls == "All") {
		SendToChart(2);
	}
	else {
		SendToChart(3);
	}

}
// set chart 
function SendToChart(requestType) {
	var cat = category.options[category.selectedIndex]?.value;
	var type = seltype.options[seltype.selectedIndex]?.value;
	var cls = selclass.options[selclass.selectedIndex]?.value;
	var results = [];

	if (requestType == 1) {
		results = FilterCategory(cat.toLowerCase());
		var response = PopulateLabels(results, "item");
		SetChart(response.label, response.quant);
	}
	else if (requestType == 2) {
		results = FilterType(type.toLowerCase());
		var response = PopulateLabels(results, "item");
		SetChart(response.label, response.quant);
	}
	else {
		results = FilterClass(cls.toLowerCase());
		var response = PopulateLabels(results, "item");
		SetChart(response.label, response.quant);
	}
}
//#endregion