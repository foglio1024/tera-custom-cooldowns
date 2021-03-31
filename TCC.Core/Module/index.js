const { TccLauncher } = require("./client/tcc-launcher");
const { TccStub } = require("./network/tcc-stub");
const { TccInterface } = require("./global/tcc-interface");

exports.ClientMod = TccLauncher;
exports.NetworkMod = TccStub;
exports.GlobalMod = TccInterface;