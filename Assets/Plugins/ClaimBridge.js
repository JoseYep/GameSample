mergeInto(LibraryManager.library,{

Claim: function(winner,loser,GameID) {

    const sha = soliditySha3(
        winner,
        loser,
        gameId
    );
    const sig = await web3.eth.accounts.sign(sha, process.env.signerPrivatekey);
    const result = await diceRoll_contract.claim(winner, loser, GameID, sig.signature, {from: winner});
    return result;
}

});