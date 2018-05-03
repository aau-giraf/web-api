var token = localStorage.getItem("token");
var username = localStorage.getItem("username");
if (token == undefined || username == undefined) {
    if (window.location != "/admin/index.html")
        window.location = "/admin/index.html";
}

function translate(errorKey) {
    if (errorKey == "UserAlreadyExists")
        return "Der eksisterer allerede en borger med dette navn.";
    if (errorKey == "MissingProperties")
        return "Udfyld venligst alle felter.";
}

var app = new Vue({
    el: '#app',
    data: {
        showWarning: false,
        ready: false,
        warningText: "",
        user: { department: null },
        citizens: [],
        citizen: { username: null }
    },
    methods: {
        init: function () {
            var self = this;
            $.ajax({
                url: "/v1/User",
                headers: { "Authorization": "Bearer " + token },
                success: function (data) {
                    self.user = data.data;
                    self.ready = true;
                }
            });
            if (window.location.href.indexOf("home"))
                self.getCitizens();
        },
        getCitizens: function () {
            var self = this;
            $.ajax({
                url: "/v1/User/" + username + "/citizens",
                headers: { "Authorization": "Bearer " + token },
                success: function (data) {
                    self.citizens = data.data;
                }
            });
        },
        addCitizen: function () {
            var self = this;
            self.citizen.DepartmentId = self.user.department;
            self.citizen.password = "password";
            $.ajax({
                url: "/v1/Account/register",
                type: 'POST',
                contentType: "application/json",
                data: JSON.stringify(self.citizen),
                headers: { "Authorization": "Bearer " + token },
                success: function (data) {
                    if (data.success)
                        window.location = "/admin/home.html";
                    else {
                        self.warningText = translate(data.errorKey);
                        self.showWarning = true;
                    }
                }
            });
        },
        logOut: function () {
            localStorage.removeItem("username");
            localStorage.removeItem("token");
            window.location = "/admin/index.html";
        },
        closeWarning: function () {
            this.showWarning = false;
        }
    }
});
app.init();