import { Handlers } from "$fresh/server.ts";
import { Lucid } from "https://deno.land/x/lucid@0.9.6/mod.ts";

function toString(arr: Uint8Array) {
    return new TextDecoder("utf-8").decode(arr);
}

export const handler: Handlers = {
    async POST(req) {
        const lucid = await Lucid.new(undefined, "Preview");
        const reqBody = await req.body?.getReader().read();
        try {
            const { address, payload, signedMessage } = JSON.parse(toString(reqBody?.value!));
            const hasSigned: boolean = lucid.verifyMessage(address, payload, signedMessage);
            return new Response(JSON.stringify({ hasSigned }), {
                headers: {
                    "Content-Type": "application/json"
                }
            });
        } catch (ex) {
            console.log(ex);
            return new Response(JSON.stringify({ hasSigned: false }), {
                headers: {
                    "Content-Type": "application/json"
                }
            });
        }
    }
};
