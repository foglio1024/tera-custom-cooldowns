const { TccStub } = require("./lib/tcc-stub");
const { TccLauncher } = require("./lib/tcc-launcher");

exports.ClientMod = TccLauncher;
exports.NetworkMod = TccStub;