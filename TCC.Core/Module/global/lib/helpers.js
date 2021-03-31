class Helpers {
    static buildHeaders(length) {
        return {
            'Content-Type': "application/json",
            'Content-Length': length,
            'User-Agent': "tcc-stub",
            'Connection': "Keep-Alive",
            'Keep-Alive': "timeout=1, max=100"
        };
    }
    static buildResponse(ret, id, type) {
        return {
            'jsonrpc': "2.0",
            [type]: ret,
            'id': id
        };
    }

    static toTitleCase(str) {
        return str.replace(
            /\w\S*/g,
            function (txt) {
                return txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase();
            }
        );
    }

    static convClass(c) {
        c = this.toTitleCase(c);
        if (c == "Elementalist") {
            c = "Mystic";
        }
        else if (c == "Engineer") {
            c = "Gunner";
        }
        else if (c == "Soulless") {
            c = "Reaper";
        }
        else if (c == "Fighter") {
            c = "Brawler";
        }
        else if (c == "Assassin") {
            c = "Ninja";
        }
        else if (c == "Glaiver") {
            c = "Valkyrie";
        }
        return c;
    }
}
exports.Helpers = Helpers;
