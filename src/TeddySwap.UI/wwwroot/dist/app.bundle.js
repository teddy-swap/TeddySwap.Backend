/******/ (() => { // webpackBootstrap
/******/ 	"use strict";
/******/ 	var __webpack_modules__ = ({

/***/ "./wwwroot/scripts/App.ts":
/*!********************************!*\
  !*** ./wwwroot/scripts/App.ts ***!
  \********************************/
/***/ ((module, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(module, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony import */ var lucid_cardano__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! lucid-cardano */ "./node_modules/lucid-cardano/esm/mod.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([lucid_cardano__WEBPACK_IMPORTED_MODULE_0__]);
lucid_cardano__WEBPACK_IMPORTED_MODULE_0__ = (__webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__)[0];

const getWallets = () => {
    const result = [];
    const cardano = window.cardano;
    for (const i in cardano) {
        const p = cardano[i];
        if (p.apiVersion != null && p.icon != null && p.name != null && i !== 'ccvault') {
            result.push({
                apiVersion: p.apiVersion,
                icon: p.icon,
                name: p.name,
                id: i.toString()
            });
        }
    }
    return result;
};
const enableAsync = async (walletId) => {
    try {
        const cardano = window.cardano;
        const walletApi = await cardano[walletId].enable();
        window.CardanoWalletService.lucid = await lucid_cardano__WEBPACK_IMPORTED_MODULE_0__.Lucid["new"](new lucid_cardano__WEBPACK_IMPORTED_MODULE_0__.Kupmios('https://kupo-preview-api-teddy-swap-preview-414e80.us1.demeter.run', 'wss://ogmios-preview-api-teddy-swap-preview-414e80.us1.demeter.run'), "Preview");
        window.CardanoWalletService.lucid.selectWallet(walletApi);
        window.CardanoWalletService.walletApi = walletApi;
    }
    catch (ex) {
        console.error(ex);
    }
    finally {
        return window.CardanoWalletService.lucid != undefined;
    }
};
const getAddressAsync = async () => {
    return await window.CardanoWalletService.lucid?.wallet.address();
};
const getUsedAddressesAsync = async () => {
    if (window.CardanoWalletService.walletApi != null) {
        const cborAddresses = await window.CardanoWalletService.walletApi.getUsedAddresses();
        const addresses = cborAddresses.map(cA => window.CardanoWalletService.lucid?.utils.getAddressDetails(cA).address.bech32).filter(cA => cA);
        return addresses;
    }
    else
        return [];
};
const signMessageAsync = async (messageHex) => {
    const rewardAddress = await getStakeAddressAsync();
    return await window.CardanoWalletService.lucid?.wallet.signMessage(rewardAddress, messageHex);
};
const getStakeAddressAsync = async () => {
    return await window.CardanoWalletService.lucid?.wallet.rewardAddress();
};
const disconnect = () => {
    delete window.CardanoWalletService.lucid;
    delete window.CardanoWalletService.walletApi;
};
window.CardanoWalletService = {
    enableAsync,
    disconnect,
    getWallets,
    getAddressAsync,
    getUsedAddressesAsync,
    signMessageAsync,
    getStakeAddressAsync
};

__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/core/wasm_modules/cardano_message_signing_web/cardano_message_signing_bg.wasm":
/*!**************************************************************************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/core/wasm_modules/cardano_message_signing_web/cardano_message_signing_bg.wasm ***!
  \**************************************************************************************************************************/
/***/ ((module, __unused_webpack_exports, __webpack_require__) => {

module.exports = __webpack_require__.p + "076fbae3fe7e20fe3173.wasm";

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/core/wasm_modules/cardano_multiplatform_lib_web/cardano_multiplatform_lib_bg.wasm":
/*!******************************************************************************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/core/wasm_modules/cardano_multiplatform_lib_web/cardano_multiplatform_lib_bg.wasm ***!
  \******************************************************************************************************************************/
/***/ ((module, __unused_webpack_exports, __webpack_require__) => {

module.exports = __webpack_require__.p + "b575e92051802f01a358.wasm";

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/deps/deno.land/std@0.100.0/encoding/hex.js":
/*!***********************************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/deps/deno.land/std@0.100.0/encoding/hex.js ***!
  \***********************************************************************************/
/***/ ((__unused_webpack___webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "decode": () => (/* binding */ decode),
/* harmony export */   "decodeString": () => (/* binding */ decodeString),
/* harmony export */   "decodedLen": () => (/* binding */ decodedLen),
/* harmony export */   "encode": () => (/* binding */ encode),
/* harmony export */   "encodeToString": () => (/* binding */ encodeToString),
/* harmony export */   "encodedLen": () => (/* binding */ encodedLen),
/* harmony export */   "errInvalidByte": () => (/* binding */ errInvalidByte),
/* harmony export */   "errLength": () => (/* binding */ errLength)
/* harmony export */ });
// Ported from Go
// https://github.com/golang/go/blob/go1.12.5/src/encoding/hex/hex.go
// Copyright 2009 The Go Authors. All rights reserved.
// Use of this source code is governed by a BSD-style
// license that can be found in the LICENSE file.
// Copyright 2018-2021 the Deno authors. All rights reserved. MIT license.
const hexTable = new TextEncoder().encode("0123456789abcdef");
/**
 * ErrInvalidByte takes an invalid byte and returns an Error.
 * @param byte
 */
function errInvalidByte(byte) {
    return new Error("encoding/hex: invalid byte: " +
        new TextDecoder().decode(new Uint8Array([byte])));
}
/** ErrLength returns an error about odd string length. */
function errLength() {
    return new Error("encoding/hex: odd length hex string");
}
// fromHexChar converts a hex character into its value.
function fromHexChar(byte) {
    // '0' <= byte && byte <= '9'
    if (48 <= byte && byte <= 57)
        return byte - 48;
    // 'a' <= byte && byte <= 'f'
    if (97 <= byte && byte <= 102)
        return byte - 97 + 10;
    // 'A' <= byte && byte <= 'F'
    if (65 <= byte && byte <= 70)
        return byte - 65 + 10;
    throw errInvalidByte(byte);
}
/**
 * EncodedLen returns the length of an encoding of n source bytes. Specifically,
 * it returns n * 2.
 * @param n
 */
function encodedLen(n) {
    return n * 2;
}
/**
 * Encode encodes `src` into `encodedLen(src.length)` bytes.
 * @param src
 */
function encode(src) {
    const dst = new Uint8Array(encodedLen(src.length));
    for (let i = 0; i < dst.length; i++) {
        const v = src[i];
        dst[i * 2] = hexTable[v >> 4];
        dst[i * 2 + 1] = hexTable[v & 0x0f];
    }
    return dst;
}
/**
 * EncodeToString returns the hexadecimal encoding of `src`.
 * @param src
 */
function encodeToString(src) {
    return new TextDecoder().decode(encode(src));
}
/**
 * Decode decodes `src` into `decodedLen(src.length)` bytes
 * If the input is malformed an error will be thrown
 * the error.
 * @param src
 */
function decode(src) {
    const dst = new Uint8Array(decodedLen(src.length));
    for (let i = 0; i < dst.length; i++) {
        const a = fromHexChar(src[i * 2]);
        const b = fromHexChar(src[i * 2 + 1]);
        dst[i] = (a << 4) | b;
    }
    if (src.length % 2 == 1) {
        // Check for invalid char before reporting bad length,
        // since the invalid char (if present) is an earlier problem.
        fromHexChar(src[dst.length * 2]);
        throw errLength();
    }
    return dst;
}
/**
 * DecodedLen returns the length of decoding `x` source bytes.
 * Specifically, it returns `x / 2`.
 * @param x
 */
function decodedLen(x) {
    return x >>> 1;
}
/**
 * DecodeString returns the bytes represented by the hexadecimal string `s`.
 * DecodeString expects that src contains only hexadecimal characters and that
 * src has even length.
 * If the input is malformed, DecodeString will throw an error.
 * @param s the `string` to decode to `Uint8Array`
 */
function decodeString(s) {
    return decode(new TextEncoder().encode(s));
}


/***/ }),

/***/ "./node_modules/lucid-cardano/esm/deps/deno.land/std@0.148.0/bytes/equals.js":
/*!***********************************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/deps/deno.land/std@0.148.0/bytes/equals.js ***!
  \***********************************************************************************/
/***/ ((__unused_webpack___webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "equals": () => (/* binding */ equals),
/* harmony export */   "equals32Bit": () => (/* binding */ equals32Bit),
/* harmony export */   "equalsNaive": () => (/* binding */ equalsNaive)
/* harmony export */ });
// Copyright 2018-2022 the Deno authors. All rights reserved. MIT license.
// This module is browser compatible.
/** Check whether binary arrays are equal to each other using 8-bit comparisons.
 * @private
 * @param a first array to check equality
 * @param b second array to check equality
 */
function equalsNaive(a, b) {
    if (a.length !== b.length)
        return false;
    for (let i = 0; i < b.length; i++) {
        if (a[i] !== b[i])
            return false;
    }
    return true;
}
/** Check whether binary arrays are equal to each other using 32-bit comparisons.
 * @private
 * @param a first array to check equality
 * @param b second array to check equality
 */
function equals32Bit(a, b) {
    if (a.length !== b.length)
        return false;
    const len = a.length;
    const compressable = Math.floor(len / 4);
    const compressedA = new Uint32Array(a.buffer, 0, compressable);
    const compressedB = new Uint32Array(b.buffer, 0, compressable);
    for (let i = compressable * 4; i < len; i++) {
        if (a[i] !== b[i])
            return false;
    }
    for (let i = 0; i < compressedA.length; i++) {
        if (compressedA[i] !== compressedB[i])
            return false;
    }
    return true;
}
/** Check whether binary arrays are equal to each other.
 * @param a first array to check equality
 * @param b second array to check equality
 */
function equals(a, b) {
    if (a.length < 1000)
        return equalsNaive(a, b);
    return equals32Bit(a, b);
}


/***/ }),

/***/ "./node_modules/lucid-cardano/esm/deps/deno.land/std@0.148.0/bytes/mod.js":
/*!********************************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/deps/deno.land/std@0.148.0/bytes/mod.js ***!
  \********************************************************************************/
/***/ ((__unused_webpack___webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "concat": () => (/* binding */ concat),
/* harmony export */   "copy": () => (/* binding */ copy),
/* harmony export */   "endsWith": () => (/* binding */ endsWith),
/* harmony export */   "equals": () => (/* reexport safe */ _equals_js__WEBPACK_IMPORTED_MODULE_0__.equals),
/* harmony export */   "includesNeedle": () => (/* binding */ includesNeedle),
/* harmony export */   "indexOfNeedle": () => (/* binding */ indexOfNeedle),
/* harmony export */   "lastIndexOfNeedle": () => (/* binding */ lastIndexOfNeedle),
/* harmony export */   "repeat": () => (/* binding */ repeat),
/* harmony export */   "startsWith": () => (/* binding */ startsWith)
/* harmony export */ });
/* harmony import */ var _equals_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./equals.js */ "./node_modules/lucid-cardano/esm/deps/deno.land/std@0.148.0/bytes/equals.js");
// Copyright 2018-2022 the Deno authors. All rights reserved. MIT license.
// This module is browser compatible.
/**
 * Provides helper functions to manipulate `Uint8Array` byte slices that are not
 * included on the `Uint8Array` prototype.
 *
 * @module
 */
/** Returns the index of the first occurrence of the needle array in the source
 * array, or -1 if it is not present.
 *
 * A start index can be specified as the third argument that begins the search
 * at that given index. The start index defaults to the start of the array.
 *
 * The complexity of this function is O(source.lenth * needle.length).
 *
 * ```ts
 * import { indexOfNeedle } from "./mod.ts";
 * const source = new Uint8Array([0, 1, 2, 1, 2, 1, 2, 3]);
 * const needle = new Uint8Array([1, 2]);
 * console.log(indexOfNeedle(source, needle)); // 1
 * console.log(indexOfNeedle(source, needle, 2)); // 3
 * ```
 */
function indexOfNeedle(source, needle, start = 0) {
    if (start >= source.length) {
        return -1;
    }
    if (start < 0) {
        start = Math.max(0, source.length + start);
    }
    const s = needle[0];
    for (let i = start; i < source.length; i++) {
        if (source[i] !== s)
            continue;
        const pin = i;
        let matched = 1;
        let j = i;
        while (matched < needle.length) {
            j++;
            if (source[j] !== needle[j - pin]) {
                break;
            }
            matched++;
        }
        if (matched === needle.length) {
            return pin;
        }
    }
    return -1;
}
/** Returns the index of the last occurrence of the needle array in the source
 * array, or -1 if it is not present.
 *
 * A start index can be specified as the third argument that begins the search
 * at that given index. The start index defaults to the end of the array.
 *
 * The complexity of this function is O(source.lenth * needle.length).
 *
 * ```ts
 * import { lastIndexOfNeedle } from "./mod.ts";
 * const source = new Uint8Array([0, 1, 2, 1, 2, 1, 2, 3]);
 * const needle = new Uint8Array([1, 2]);
 * console.log(lastIndexOfNeedle(source, needle)); // 5
 * console.log(lastIndexOfNeedle(source, needle, 4)); // 3
 * ```
 */
function lastIndexOfNeedle(source, needle, start = source.length - 1) {
    if (start < 0) {
        return -1;
    }
    if (start >= source.length) {
        start = source.length - 1;
    }
    const e = needle[needle.length - 1];
    for (let i = start; i >= 0; i--) {
        if (source[i] !== e)
            continue;
        const pin = i;
        let matched = 1;
        let j = i;
        while (matched < needle.length) {
            j--;
            if (source[j] !== needle[needle.length - 1 - (pin - j)]) {
                break;
            }
            matched++;
        }
        if (matched === needle.length) {
            return pin - needle.length + 1;
        }
    }
    return -1;
}
/** Returns true if the prefix array appears at the start of the source array,
 * false otherwise.
 *
 * The complexity of this function is O(prefix.length).
 *
 * ```ts
 * import { startsWith } from "./mod.ts";
 * const source = new Uint8Array([0, 1, 2, 1, 2, 1, 2, 3]);
 * const prefix = new Uint8Array([0, 1, 2]);
 * console.log(startsWith(source, prefix)); // true
 * ```
 */
function startsWith(source, prefix) {
    for (let i = 0, max = prefix.length; i < max; i++) {
        if (source[i] !== prefix[i])
            return false;
    }
    return true;
}
/** Returns true if the suffix array appears at the end of the source array,
 * false otherwise.
 *
 * The complexity of this function is O(suffix.length).
 *
 * ```ts
 * import { endsWith } from "./mod.ts";
 * const source = new Uint8Array([0, 1, 2, 1, 2, 1, 2, 3]);
 * const suffix = new Uint8Array([1, 2, 3]);
 * console.log(endsWith(source, suffix)); // true
 * ```
 */
function endsWith(source, suffix) {
    for (let srci = source.length - 1, sfxi = suffix.length - 1; sfxi >= 0; srci--, sfxi--) {
        if (source[srci] !== suffix[sfxi])
            return false;
    }
    return true;
}
/** Returns a new Uint8Array composed of `count` repetitions of the `source`
 * array.
 *
 * If `count` is negative, a `RangeError` is thrown.
 *
 * ```ts
 * import { repeat } from "./mod.ts";
 * const source = new Uint8Array([0, 1, 2]);
 * console.log(repeat(source, 3)); // [0, 1, 2, 0, 1, 2, 0, 1, 2]
 * console.log(repeat(source, 0)); // []
 * console.log(repeat(source, -1)); // RangeError
 * ```
 */
function repeat(source, count) {
    if (count === 0) {
        return new Uint8Array();
    }
    if (count < 0) {
        throw new RangeError("bytes: negative repeat count");
    }
    else if ((source.length * count) / count !== source.length) {
        throw new Error("bytes: repeat count causes overflow");
    }
    const int = Math.floor(count);
    if (int !== count) {
        throw new Error("bytes: repeat count must be an integer");
    }
    const nb = new Uint8Array(source.length * count);
    let bp = copy(source, nb);
    for (; bp < nb.length; bp *= 2) {
        copy(nb.slice(0, bp), nb, bp);
    }
    return nb;
}
/** Concatenate the given arrays into a new Uint8Array.
 *
 * ```ts
 * import { concat } from "./mod.ts";
 * const a = new Uint8Array([0, 1, 2]);
 * const b = new Uint8Array([3, 4, 5]);
 * console.log(concat(a, b)); // [0, 1, 2, 3, 4, 5]
 */
function concat(...buf) {
    let length = 0;
    for (const b of buf) {
        length += b.length;
    }
    const output = new Uint8Array(length);
    let index = 0;
    for (const b of buf) {
        output.set(b, index);
        index += b.length;
    }
    return output;
}
/** Returns true if the source array contains the needle array, false otherwise.
 *
 * A start index can be specified as the third argument that begins the search
 * at that given index. The start index defaults to the beginning of the array.
 *
 * The complexity of this function is O(source.length * needle.length).
 *
 * ```ts
 * import { includesNeedle } from "./mod.ts";
 * const source = new Uint8Array([0, 1, 2, 1, 2, 1, 2, 3]);
 * const needle = new Uint8Array([1, 2]);
 * console.log(includesNeedle(source, needle)); // true
 * console.log(includesNeedle(source, needle, 6)); // false
 * ```
 */
function includesNeedle(source, needle, start = 0) {
    return indexOfNeedle(source, needle, start) !== -1;
}
/** Copy bytes from the `src` array to the `dst` array. Returns the number of
 * bytes copied.
 *
 * If the `src` array is larger than what the `dst` array can hold, only the
 * amount of bytes that fit in the `dst` array are copied.
 *
 * An offset can be specified as the third argument that begins the copy at
 * that given index in the `dst` array. The offset defaults to the beginning of
 * the array.
 *
 * ```ts
 * import { copy } from "./mod.ts";
 * const src = new Uint8Array([9, 8, 7]);
 * const dst = new Uint8Array([0, 1, 2, 3, 4, 5]);
 * console.log(copy(src, dst)); // 3
 * console.log(dst); // [9, 8, 7, 3, 4, 5]
 * ```
 *
 * ```ts
 * import { copy } from "./mod.ts";
 * const src = new Uint8Array([1, 1, 1, 1]);
 * const dst = new Uint8Array([0, 0, 0, 0]);
 * console.log(copy(src, dst, 1)); // 3
 * console.log(dst); // [0, 1, 1, 1]
 * ```
 */
function copy(src, dst, off = 0) {
    off = Math.max(0, Math.min(off, dst.byteLength));
    const dstBytesAvailable = dst.byteLength - off;
    if (src.byteLength > dstBytesAvailable) {
        src = src.subarray(0, dstBytesAvailable);
    }
    dst.set(src, off);
    return src.byteLength;
}



/***/ }),

/***/ "./node_modules/lucid-cardano/esm/deps/deno.land/std@0.153.0/hash/sha256.js":
/*!**********************************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/deps/deno.land/std@0.153.0/hash/sha256.js ***!
  \**********************************************************************************/
/***/ ((__unused_webpack___webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "HmacSha256": () => (/* binding */ HmacSha256),
/* harmony export */   "Sha256": () => (/* binding */ Sha256)
/* harmony export */ });
// Copyright 2018-2022 the Deno authors. All rights reserved. MIT license.
// This module is browser compatible.
var __classPrivateFieldSet = (undefined && undefined.__classPrivateFieldSet) || function (receiver, state, value, kind, f) {
    if (kind === "m") throw new TypeError("Private method is not writable");
    if (kind === "a" && !f) throw new TypeError("Private accessor was defined without a setter");
    if (typeof state === "function" ? receiver !== state || !f : !state.has(receiver)) throw new TypeError("Cannot write private member to an object whose class did not declare it");
    return (kind === "a" ? f.call(receiver, value) : f ? f.value = value : state.set(receiver, value)), value;
};
var __classPrivateFieldGet = (undefined && undefined.__classPrivateFieldGet) || function (receiver, state, kind, f) {
    if (kind === "a" && !f) throw new TypeError("Private accessor was defined without a getter");
    if (typeof state === "function" ? receiver !== state || !f : !state.has(receiver)) throw new TypeError("Cannot read private member from an object whose class did not declare it");
    return kind === "m" ? f : kind === "a" ? f.call(receiver) : f ? f.value : state.get(receiver);
};
var _Sha256_block, _Sha256_blocks, _Sha256_bytes, _Sha256_finalized, _Sha256_first, _Sha256_h0, _Sha256_h1, _Sha256_h2, _Sha256_h3, _Sha256_h4, _Sha256_h5, _Sha256_h6, _Sha256_h7, _Sha256_hashed, _Sha256_hBytes, _Sha256_is224, _Sha256_lastByteIndex, _Sha256_start, _HmacSha256_inner, _HmacSha256_is224, _HmacSha256_oKeyPad, _HmacSha256_sharedMemory;
const HEX_CHARS = "0123456789abcdef".split("");
const EXTRA = [-2147483648, 8388608, 32768, 128];
const SHIFT = [24, 16, 8, 0];
const K = [
    0x428a2f98,
    0x71374491,
    0xb5c0fbcf,
    0xe9b5dba5,
    0x3956c25b,
    0x59f111f1,
    0x923f82a4,
    0xab1c5ed5,
    0xd807aa98,
    0x12835b01,
    0x243185be,
    0x550c7dc3,
    0x72be5d74,
    0x80deb1fe,
    0x9bdc06a7,
    0xc19bf174,
    0xe49b69c1,
    0xefbe4786,
    0x0fc19dc6,
    0x240ca1cc,
    0x2de92c6f,
    0x4a7484aa,
    0x5cb0a9dc,
    0x76f988da,
    0x983e5152,
    0xa831c66d,
    0xb00327c8,
    0xbf597fc7,
    0xc6e00bf3,
    0xd5a79147,
    0x06ca6351,
    0x14292967,
    0x27b70a85,
    0x2e1b2138,
    0x4d2c6dfc,
    0x53380d13,
    0x650a7354,
    0x766a0abb,
    0x81c2c92e,
    0x92722c85,
    0xa2bfe8a1,
    0xa81a664b,
    0xc24b8b70,
    0xc76c51a3,
    0xd192e819,
    0xd6990624,
    0xf40e3585,
    0x106aa070,
    0x19a4c116,
    0x1e376c08,
    0x2748774c,
    0x34b0bcb5,
    0x391c0cb3,
    0x4ed8aa4a,
    0x5b9cca4f,
    0x682e6ff3,
    0x748f82ee,
    0x78a5636f,
    0x84c87814,
    0x8cc70208,
    0x90befffa,
    0xa4506ceb,
    0xbef9a3f7,
    0xc67178f2,
];
const blocks = [];
class Sha256 {
    constructor(is224 = false, sharedMemory = false) {
        _Sha256_block.set(this, void 0);
        _Sha256_blocks.set(this, void 0);
        _Sha256_bytes.set(this, void 0);
        _Sha256_finalized.set(this, void 0);
        _Sha256_first.set(this, void 0);
        _Sha256_h0.set(this, void 0);
        _Sha256_h1.set(this, void 0);
        _Sha256_h2.set(this, void 0);
        _Sha256_h3.set(this, void 0);
        _Sha256_h4.set(this, void 0);
        _Sha256_h5.set(this, void 0);
        _Sha256_h6.set(this, void 0);
        _Sha256_h7.set(this, void 0);
        _Sha256_hashed.set(this, void 0);
        _Sha256_hBytes.set(this, void 0);
        _Sha256_is224.set(this, void 0);
        _Sha256_lastByteIndex.set(this, 0);
        _Sha256_start.set(this, void 0);
        this.init(is224, sharedMemory);
    }
    init(is224, sharedMemory) {
        if (sharedMemory) {
            blocks[0] =
                blocks[16] =
                    blocks[1] =
                        blocks[2] =
                            blocks[3] =
                                blocks[4] =
                                    blocks[5] =
                                        blocks[6] =
                                            blocks[7] =
                                                blocks[8] =
                                                    blocks[9] =
                                                        blocks[10] =
                                                            blocks[11] =
                                                                blocks[12] =
                                                                    blocks[13] =
                                                                        blocks[14] =
                                                                            blocks[15] =
                                                                                0;
            __classPrivateFieldSet(this, _Sha256_blocks, blocks, "f");
        }
        else {
            __classPrivateFieldSet(this, _Sha256_blocks, [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0], "f");
        }
        if (is224) {
            __classPrivateFieldSet(this, _Sha256_h0, 0xc1059ed8, "f");
            __classPrivateFieldSet(this, _Sha256_h1, 0x367cd507, "f");
            __classPrivateFieldSet(this, _Sha256_h2, 0x3070dd17, "f");
            __classPrivateFieldSet(this, _Sha256_h3, 0xf70e5939, "f");
            __classPrivateFieldSet(this, _Sha256_h4, 0xffc00b31, "f");
            __classPrivateFieldSet(this, _Sha256_h5, 0x68581511, "f");
            __classPrivateFieldSet(this, _Sha256_h6, 0x64f98fa7, "f");
            __classPrivateFieldSet(this, _Sha256_h7, 0xbefa4fa4, "f");
        }
        else {
            // 256
            __classPrivateFieldSet(this, _Sha256_h0, 0x6a09e667, "f");
            __classPrivateFieldSet(this, _Sha256_h1, 0xbb67ae85, "f");
            __classPrivateFieldSet(this, _Sha256_h2, 0x3c6ef372, "f");
            __classPrivateFieldSet(this, _Sha256_h3, 0xa54ff53a, "f");
            __classPrivateFieldSet(this, _Sha256_h4, 0x510e527f, "f");
            __classPrivateFieldSet(this, _Sha256_h5, 0x9b05688c, "f");
            __classPrivateFieldSet(this, _Sha256_h6, 0x1f83d9ab, "f");
            __classPrivateFieldSet(this, _Sha256_h7, 0x5be0cd19, "f");
        }
        __classPrivateFieldSet(this, _Sha256_block, __classPrivateFieldSet(this, _Sha256_start, __classPrivateFieldSet(this, _Sha256_bytes, __classPrivateFieldSet(this, _Sha256_hBytes, 0, "f"), "f"), "f"), "f");
        __classPrivateFieldSet(this, _Sha256_finalized, __classPrivateFieldSet(this, _Sha256_hashed, false, "f"), "f");
        __classPrivateFieldSet(this, _Sha256_first, true, "f");
        __classPrivateFieldSet(this, _Sha256_is224, is224, "f");
    }
    /** Update hash
     *
     * @param message The message you want to hash.
     */
    update(message) {
        if (__classPrivateFieldGet(this, _Sha256_finalized, "f")) {
            return this;
        }
        let msg;
        if (message instanceof ArrayBuffer) {
            msg = new Uint8Array(message);
        }
        else {
            msg = message;
        }
        let index = 0;
        const length = msg.length;
        const blocks = __classPrivateFieldGet(this, _Sha256_blocks, "f");
        while (index < length) {
            let i;
            if (__classPrivateFieldGet(this, _Sha256_hashed, "f")) {
                __classPrivateFieldSet(this, _Sha256_hashed, false, "f");
                blocks[0] = __classPrivateFieldGet(this, _Sha256_block, "f");
                blocks[16] =
                    blocks[1] =
                        blocks[2] =
                            blocks[3] =
                                blocks[4] =
                                    blocks[5] =
                                        blocks[6] =
                                            blocks[7] =
                                                blocks[8] =
                                                    blocks[9] =
                                                        blocks[10] =
                                                            blocks[11] =
                                                                blocks[12] =
                                                                    blocks[13] =
                                                                        blocks[14] =
                                                                            blocks[15] =
                                                                                0;
            }
            if (typeof msg !== "string") {
                for (i = __classPrivateFieldGet(this, _Sha256_start, "f"); index < length && i < 64; ++index) {
                    blocks[i >> 2] |= msg[index] << SHIFT[i++ & 3];
                }
            }
            else {
                for (i = __classPrivateFieldGet(this, _Sha256_start, "f"); index < length && i < 64; ++index) {
                    let code = msg.charCodeAt(index);
                    if (code < 0x80) {
                        blocks[i >> 2] |= code << SHIFT[i++ & 3];
                    }
                    else if (code < 0x800) {
                        blocks[i >> 2] |= (0xc0 | (code >> 6)) << SHIFT[i++ & 3];
                        blocks[i >> 2] |= (0x80 | (code & 0x3f)) << SHIFT[i++ & 3];
                    }
                    else if (code < 0xd800 || code >= 0xe000) {
                        blocks[i >> 2] |= (0xe0 | (code >> 12)) << SHIFT[i++ & 3];
                        blocks[i >> 2] |= (0x80 | ((code >> 6) & 0x3f)) << SHIFT[i++ & 3];
                        blocks[i >> 2] |= (0x80 | (code & 0x3f)) << SHIFT[i++ & 3];
                    }
                    else {
                        code = 0x10000 +
                            (((code & 0x3ff) << 10) | (msg.charCodeAt(++index) & 0x3ff));
                        blocks[i >> 2] |= (0xf0 | (code >> 18)) << SHIFT[i++ & 3];
                        blocks[i >> 2] |= (0x80 | ((code >> 12) & 0x3f)) << SHIFT[i++ & 3];
                        blocks[i >> 2] |= (0x80 | ((code >> 6) & 0x3f)) << SHIFT[i++ & 3];
                        blocks[i >> 2] |= (0x80 | (code & 0x3f)) << SHIFT[i++ & 3];
                    }
                }
            }
            __classPrivateFieldSet(this, _Sha256_lastByteIndex, i, "f");
            __classPrivateFieldSet(this, _Sha256_bytes, __classPrivateFieldGet(this, _Sha256_bytes, "f") + (i - __classPrivateFieldGet(this, _Sha256_start, "f")), "f");
            if (i >= 64) {
                __classPrivateFieldSet(this, _Sha256_block, blocks[16], "f");
                __classPrivateFieldSet(this, _Sha256_start, i - 64, "f");
                this.hash();
                __classPrivateFieldSet(this, _Sha256_hashed, true, "f");
            }
            else {
                __classPrivateFieldSet(this, _Sha256_start, i, "f");
            }
        }
        if (__classPrivateFieldGet(this, _Sha256_bytes, "f") > 4294967295) {
            __classPrivateFieldSet(this, _Sha256_hBytes, __classPrivateFieldGet(this, _Sha256_hBytes, "f") + ((__classPrivateFieldGet(this, _Sha256_bytes, "f") / 4294967296) << 0), "f");
            __classPrivateFieldSet(this, _Sha256_bytes, __classPrivateFieldGet(this, _Sha256_bytes, "f") % 4294967296, "f");
        }
        return this;
    }
    finalize() {
        if (__classPrivateFieldGet(this, _Sha256_finalized, "f")) {
            return;
        }
        __classPrivateFieldSet(this, _Sha256_finalized, true, "f");
        const blocks = __classPrivateFieldGet(this, _Sha256_blocks, "f");
        const i = __classPrivateFieldGet(this, _Sha256_lastByteIndex, "f");
        blocks[16] = __classPrivateFieldGet(this, _Sha256_block, "f");
        blocks[i >> 2] |= EXTRA[i & 3];
        __classPrivateFieldSet(this, _Sha256_block, blocks[16], "f");
        if (i >= 56) {
            if (!__classPrivateFieldGet(this, _Sha256_hashed, "f")) {
                this.hash();
            }
            blocks[0] = __classPrivateFieldGet(this, _Sha256_block, "f");
            blocks[16] =
                blocks[1] =
                    blocks[2] =
                        blocks[3] =
                            blocks[4] =
                                blocks[5] =
                                    blocks[6] =
                                        blocks[7] =
                                            blocks[8] =
                                                blocks[9] =
                                                    blocks[10] =
                                                        blocks[11] =
                                                            blocks[12] =
                                                                blocks[13] =
                                                                    blocks[14] =
                                                                        blocks[15] =
                                                                            0;
        }
        blocks[14] = (__classPrivateFieldGet(this, _Sha256_hBytes, "f") << 3) | (__classPrivateFieldGet(this, _Sha256_bytes, "f") >>> 29);
        blocks[15] = __classPrivateFieldGet(this, _Sha256_bytes, "f") << 3;
        this.hash();
    }
    hash() {
        let a = __classPrivateFieldGet(this, _Sha256_h0, "f");
        let b = __classPrivateFieldGet(this, _Sha256_h1, "f");
        let c = __classPrivateFieldGet(this, _Sha256_h2, "f");
        let d = __classPrivateFieldGet(this, _Sha256_h3, "f");
        let e = __classPrivateFieldGet(this, _Sha256_h4, "f");
        let f = __classPrivateFieldGet(this, _Sha256_h5, "f");
        let g = __classPrivateFieldGet(this, _Sha256_h6, "f");
        let h = __classPrivateFieldGet(this, _Sha256_h7, "f");
        const blocks = __classPrivateFieldGet(this, _Sha256_blocks, "f");
        let s0;
        let s1;
        let maj;
        let t1;
        let t2;
        let ch;
        let ab;
        let da;
        let cd;
        let bc;
        for (let j = 16; j < 64; ++j) {
            // rightrotate
            t1 = blocks[j - 15];
            s0 = ((t1 >>> 7) | (t1 << 25)) ^ ((t1 >>> 18) | (t1 << 14)) ^ (t1 >>> 3);
            t1 = blocks[j - 2];
            s1 = ((t1 >>> 17) | (t1 << 15)) ^ ((t1 >>> 19) | (t1 << 13)) ^
                (t1 >>> 10);
            blocks[j] = (blocks[j - 16] + s0 + blocks[j - 7] + s1) << 0;
        }
        bc = b & c;
        for (let j = 0; j < 64; j += 4) {
            if (__classPrivateFieldGet(this, _Sha256_first, "f")) {
                if (__classPrivateFieldGet(this, _Sha256_is224, "f")) {
                    ab = 300032;
                    t1 = blocks[0] - 1413257819;
                    h = (t1 - 150054599) << 0;
                    d = (t1 + 24177077) << 0;
                }
                else {
                    ab = 704751109;
                    t1 = blocks[0] - 210244248;
                    h = (t1 - 1521486534) << 0;
                    d = (t1 + 143694565) << 0;
                }
                __classPrivateFieldSet(this, _Sha256_first, false, "f");
            }
            else {
                s0 = ((a >>> 2) | (a << 30)) ^
                    ((a >>> 13) | (a << 19)) ^
                    ((a >>> 22) | (a << 10));
                s1 = ((e >>> 6) | (e << 26)) ^
                    ((e >>> 11) | (e << 21)) ^
                    ((e >>> 25) | (e << 7));
                ab = a & b;
                maj = ab ^ (a & c) ^ bc;
                ch = (e & f) ^ (~e & g);
                t1 = h + s1 + ch + K[j] + blocks[j];
                t2 = s0 + maj;
                h = (d + t1) << 0;
                d = (t1 + t2) << 0;
            }
            s0 = ((d >>> 2) | (d << 30)) ^
                ((d >>> 13) | (d << 19)) ^
                ((d >>> 22) | (d << 10));
            s1 = ((h >>> 6) | (h << 26)) ^
                ((h >>> 11) | (h << 21)) ^
                ((h >>> 25) | (h << 7));
            da = d & a;
            maj = da ^ (d & b) ^ ab;
            ch = (h & e) ^ (~h & f);
            t1 = g + s1 + ch + K[j + 1] + blocks[j + 1];
            t2 = s0 + maj;
            g = (c + t1) << 0;
            c = (t1 + t2) << 0;
            s0 = ((c >>> 2) | (c << 30)) ^
                ((c >>> 13) | (c << 19)) ^
                ((c >>> 22) | (c << 10));
            s1 = ((g >>> 6) | (g << 26)) ^
                ((g >>> 11) | (g << 21)) ^
                ((g >>> 25) | (g << 7));
            cd = c & d;
            maj = cd ^ (c & a) ^ da;
            ch = (g & h) ^ (~g & e);
            t1 = f + s1 + ch + K[j + 2] + blocks[j + 2];
            t2 = s0 + maj;
            f = (b + t1) << 0;
            b = (t1 + t2) << 0;
            s0 = ((b >>> 2) | (b << 30)) ^
                ((b >>> 13) | (b << 19)) ^
                ((b >>> 22) | (b << 10));
            s1 = ((f >>> 6) | (f << 26)) ^
                ((f >>> 11) | (f << 21)) ^
                ((f >>> 25) | (f << 7));
            bc = b & c;
            maj = bc ^ (b & d) ^ cd;
            ch = (f & g) ^ (~f & h);
            t1 = e + s1 + ch + K[j + 3] + blocks[j + 3];
            t2 = s0 + maj;
            e = (a + t1) << 0;
            a = (t1 + t2) << 0;
        }
        __classPrivateFieldSet(this, _Sha256_h0, (__classPrivateFieldGet(this, _Sha256_h0, "f") + a) << 0, "f");
        __classPrivateFieldSet(this, _Sha256_h1, (__classPrivateFieldGet(this, _Sha256_h1, "f") + b) << 0, "f");
        __classPrivateFieldSet(this, _Sha256_h2, (__classPrivateFieldGet(this, _Sha256_h2, "f") + c) << 0, "f");
        __classPrivateFieldSet(this, _Sha256_h3, (__classPrivateFieldGet(this, _Sha256_h3, "f") + d) << 0, "f");
        __classPrivateFieldSet(this, _Sha256_h4, (__classPrivateFieldGet(this, _Sha256_h4, "f") + e) << 0, "f");
        __classPrivateFieldSet(this, _Sha256_h5, (__classPrivateFieldGet(this, _Sha256_h5, "f") + f) << 0, "f");
        __classPrivateFieldSet(this, _Sha256_h6, (__classPrivateFieldGet(this, _Sha256_h6, "f") + g) << 0, "f");
        __classPrivateFieldSet(this, _Sha256_h7, (__classPrivateFieldGet(this, _Sha256_h7, "f") + h) << 0, "f");
    }
    /** Return hash in hex string. */
    hex() {
        this.finalize();
        const h0 = __classPrivateFieldGet(this, _Sha256_h0, "f");
        const h1 = __classPrivateFieldGet(this, _Sha256_h1, "f");
        const h2 = __classPrivateFieldGet(this, _Sha256_h2, "f");
        const h3 = __classPrivateFieldGet(this, _Sha256_h3, "f");
        const h4 = __classPrivateFieldGet(this, _Sha256_h4, "f");
        const h5 = __classPrivateFieldGet(this, _Sha256_h5, "f");
        const h6 = __classPrivateFieldGet(this, _Sha256_h6, "f");
        const h7 = __classPrivateFieldGet(this, _Sha256_h7, "f");
        let hex = HEX_CHARS[(h0 >> 28) & 0x0f] +
            HEX_CHARS[(h0 >> 24) & 0x0f] +
            HEX_CHARS[(h0 >> 20) & 0x0f] +
            HEX_CHARS[(h0 >> 16) & 0x0f] +
            HEX_CHARS[(h0 >> 12) & 0x0f] +
            HEX_CHARS[(h0 >> 8) & 0x0f] +
            HEX_CHARS[(h0 >> 4) & 0x0f] +
            HEX_CHARS[h0 & 0x0f] +
            HEX_CHARS[(h1 >> 28) & 0x0f] +
            HEX_CHARS[(h1 >> 24) & 0x0f] +
            HEX_CHARS[(h1 >> 20) & 0x0f] +
            HEX_CHARS[(h1 >> 16) & 0x0f] +
            HEX_CHARS[(h1 >> 12) & 0x0f] +
            HEX_CHARS[(h1 >> 8) & 0x0f] +
            HEX_CHARS[(h1 >> 4) & 0x0f] +
            HEX_CHARS[h1 & 0x0f] +
            HEX_CHARS[(h2 >> 28) & 0x0f] +
            HEX_CHARS[(h2 >> 24) & 0x0f] +
            HEX_CHARS[(h2 >> 20) & 0x0f] +
            HEX_CHARS[(h2 >> 16) & 0x0f] +
            HEX_CHARS[(h2 >> 12) & 0x0f] +
            HEX_CHARS[(h2 >> 8) & 0x0f] +
            HEX_CHARS[(h2 >> 4) & 0x0f] +
            HEX_CHARS[h2 & 0x0f] +
            HEX_CHARS[(h3 >> 28) & 0x0f] +
            HEX_CHARS[(h3 >> 24) & 0x0f] +
            HEX_CHARS[(h3 >> 20) & 0x0f] +
            HEX_CHARS[(h3 >> 16) & 0x0f] +
            HEX_CHARS[(h3 >> 12) & 0x0f] +
            HEX_CHARS[(h3 >> 8) & 0x0f] +
            HEX_CHARS[(h3 >> 4) & 0x0f] +
            HEX_CHARS[h3 & 0x0f] +
            HEX_CHARS[(h4 >> 28) & 0x0f] +
            HEX_CHARS[(h4 >> 24) & 0x0f] +
            HEX_CHARS[(h4 >> 20) & 0x0f] +
            HEX_CHARS[(h4 >> 16) & 0x0f] +
            HEX_CHARS[(h4 >> 12) & 0x0f] +
            HEX_CHARS[(h4 >> 8) & 0x0f] +
            HEX_CHARS[(h4 >> 4) & 0x0f] +
            HEX_CHARS[h4 & 0x0f] +
            HEX_CHARS[(h5 >> 28) & 0x0f] +
            HEX_CHARS[(h5 >> 24) & 0x0f] +
            HEX_CHARS[(h5 >> 20) & 0x0f] +
            HEX_CHARS[(h5 >> 16) & 0x0f] +
            HEX_CHARS[(h5 >> 12) & 0x0f] +
            HEX_CHARS[(h5 >> 8) & 0x0f] +
            HEX_CHARS[(h5 >> 4) & 0x0f] +
            HEX_CHARS[h5 & 0x0f] +
            HEX_CHARS[(h6 >> 28) & 0x0f] +
            HEX_CHARS[(h6 >> 24) & 0x0f] +
            HEX_CHARS[(h6 >> 20) & 0x0f] +
            HEX_CHARS[(h6 >> 16) & 0x0f] +
            HEX_CHARS[(h6 >> 12) & 0x0f] +
            HEX_CHARS[(h6 >> 8) & 0x0f] +
            HEX_CHARS[(h6 >> 4) & 0x0f] +
            HEX_CHARS[h6 & 0x0f];
        if (!__classPrivateFieldGet(this, _Sha256_is224, "f")) {
            hex += HEX_CHARS[(h7 >> 28) & 0x0f] +
                HEX_CHARS[(h7 >> 24) & 0x0f] +
                HEX_CHARS[(h7 >> 20) & 0x0f] +
                HEX_CHARS[(h7 >> 16) & 0x0f] +
                HEX_CHARS[(h7 >> 12) & 0x0f] +
                HEX_CHARS[(h7 >> 8) & 0x0f] +
                HEX_CHARS[(h7 >> 4) & 0x0f] +
                HEX_CHARS[h7 & 0x0f];
        }
        return hex;
    }
    /** Return hash in hex string. */
    toString() {
        return this.hex();
    }
    /** Return hash in integer array. */
    digest() {
        this.finalize();
        const h0 = __classPrivateFieldGet(this, _Sha256_h0, "f");
        const h1 = __classPrivateFieldGet(this, _Sha256_h1, "f");
        const h2 = __classPrivateFieldGet(this, _Sha256_h2, "f");
        const h3 = __classPrivateFieldGet(this, _Sha256_h3, "f");
        const h4 = __classPrivateFieldGet(this, _Sha256_h4, "f");
        const h5 = __classPrivateFieldGet(this, _Sha256_h5, "f");
        const h6 = __classPrivateFieldGet(this, _Sha256_h6, "f");
        const h7 = __classPrivateFieldGet(this, _Sha256_h7, "f");
        const arr = [
            (h0 >> 24) & 0xff,
            (h0 >> 16) & 0xff,
            (h0 >> 8) & 0xff,
            h0 & 0xff,
            (h1 >> 24) & 0xff,
            (h1 >> 16) & 0xff,
            (h1 >> 8) & 0xff,
            h1 & 0xff,
            (h2 >> 24) & 0xff,
            (h2 >> 16) & 0xff,
            (h2 >> 8) & 0xff,
            h2 & 0xff,
            (h3 >> 24) & 0xff,
            (h3 >> 16) & 0xff,
            (h3 >> 8) & 0xff,
            h3 & 0xff,
            (h4 >> 24) & 0xff,
            (h4 >> 16) & 0xff,
            (h4 >> 8) & 0xff,
            h4 & 0xff,
            (h5 >> 24) & 0xff,
            (h5 >> 16) & 0xff,
            (h5 >> 8) & 0xff,
            h5 & 0xff,
            (h6 >> 24) & 0xff,
            (h6 >> 16) & 0xff,
            (h6 >> 8) & 0xff,
            h6 & 0xff,
        ];
        if (!__classPrivateFieldGet(this, _Sha256_is224, "f")) {
            arr.push((h7 >> 24) & 0xff, (h7 >> 16) & 0xff, (h7 >> 8) & 0xff, h7 & 0xff);
        }
        return arr;
    }
    /** Return hash in integer array. */
    array() {
        return this.digest();
    }
    /** Return hash in ArrayBuffer. */
    arrayBuffer() {
        this.finalize();
        const buffer = new ArrayBuffer(__classPrivateFieldGet(this, _Sha256_is224, "f") ? 28 : 32);
        const dataView = new DataView(buffer);
        dataView.setUint32(0, __classPrivateFieldGet(this, _Sha256_h0, "f"));
        dataView.setUint32(4, __classPrivateFieldGet(this, _Sha256_h1, "f"));
        dataView.setUint32(8, __classPrivateFieldGet(this, _Sha256_h2, "f"));
        dataView.setUint32(12, __classPrivateFieldGet(this, _Sha256_h3, "f"));
        dataView.setUint32(16, __classPrivateFieldGet(this, _Sha256_h4, "f"));
        dataView.setUint32(20, __classPrivateFieldGet(this, _Sha256_h5, "f"));
        dataView.setUint32(24, __classPrivateFieldGet(this, _Sha256_h6, "f"));
        if (!__classPrivateFieldGet(this, _Sha256_is224, "f")) {
            dataView.setUint32(28, __classPrivateFieldGet(this, _Sha256_h7, "f"));
        }
        return buffer;
    }
}
_Sha256_block = new WeakMap(), _Sha256_blocks = new WeakMap(), _Sha256_bytes = new WeakMap(), _Sha256_finalized = new WeakMap(), _Sha256_first = new WeakMap(), _Sha256_h0 = new WeakMap(), _Sha256_h1 = new WeakMap(), _Sha256_h2 = new WeakMap(), _Sha256_h3 = new WeakMap(), _Sha256_h4 = new WeakMap(), _Sha256_h5 = new WeakMap(), _Sha256_h6 = new WeakMap(), _Sha256_h7 = new WeakMap(), _Sha256_hashed = new WeakMap(), _Sha256_hBytes = new WeakMap(), _Sha256_is224 = new WeakMap(), _Sha256_lastByteIndex = new WeakMap(), _Sha256_start = new WeakMap();
class HmacSha256 extends Sha256 {
    constructor(secretKey, is224 = false, sharedMemory = false) {
        super(is224, sharedMemory);
        _HmacSha256_inner.set(this, void 0);
        _HmacSha256_is224.set(this, void 0);
        _HmacSha256_oKeyPad.set(this, void 0);
        _HmacSha256_sharedMemory.set(this, void 0);
        let key;
        if (typeof secretKey === "string") {
            const bytes = [];
            const length = secretKey.length;
            let index = 0;
            for (let i = 0; i < length; ++i) {
                let code = secretKey.charCodeAt(i);
                if (code < 0x80) {
                    bytes[index++] = code;
                }
                else if (code < 0x800) {
                    bytes[index++] = 0xc0 | (code >> 6);
                    bytes[index++] = 0x80 | (code & 0x3f);
                }
                else if (code < 0xd800 || code >= 0xe000) {
                    bytes[index++] = 0xe0 | (code >> 12);
                    bytes[index++] = 0x80 | ((code >> 6) & 0x3f);
                    bytes[index++] = 0x80 | (code & 0x3f);
                }
                else {
                    code = 0x10000 +
                        (((code & 0x3ff) << 10) | (secretKey.charCodeAt(++i) & 0x3ff));
                    bytes[index++] = 0xf0 | (code >> 18);
                    bytes[index++] = 0x80 | ((code >> 12) & 0x3f);
                    bytes[index++] = 0x80 | ((code >> 6) & 0x3f);
                    bytes[index++] = 0x80 | (code & 0x3f);
                }
            }
            key = bytes;
        }
        else {
            if (secretKey instanceof ArrayBuffer) {
                key = new Uint8Array(secretKey);
            }
            else {
                key = secretKey;
            }
        }
        if (key.length > 64) {
            key = new Sha256(is224, true).update(key).array();
        }
        const oKeyPad = [];
        const iKeyPad = [];
        for (let i = 0; i < 64; ++i) {
            const b = key[i] || 0;
            oKeyPad[i] = 0x5c ^ b;
            iKeyPad[i] = 0x36 ^ b;
        }
        this.update(iKeyPad);
        __classPrivateFieldSet(this, _HmacSha256_oKeyPad, oKeyPad, "f");
        __classPrivateFieldSet(this, _HmacSha256_inner, true, "f");
        __classPrivateFieldSet(this, _HmacSha256_is224, is224, "f");
        __classPrivateFieldSet(this, _HmacSha256_sharedMemory, sharedMemory, "f");
    }
    finalize() {
        super.finalize();
        if (__classPrivateFieldGet(this, _HmacSha256_inner, "f")) {
            __classPrivateFieldSet(this, _HmacSha256_inner, false, "f");
            const innerHash = this.array();
            super.init(__classPrivateFieldGet(this, _HmacSha256_is224, "f"), __classPrivateFieldGet(this, _HmacSha256_sharedMemory, "f"));
            this.update(__classPrivateFieldGet(this, _HmacSha256_oKeyPad, "f"));
            this.update(innerHash);
            super.finalize();
        }
    }
}
_HmacSha256_inner = new WeakMap(), _HmacSha256_is224 = new WeakMap(), _HmacSha256_oKeyPad = new WeakMap(), _HmacSha256_sharedMemory = new WeakMap();


/***/ }),

/***/ "./node_modules/lucid-cardano/esm/deps/deno.land/x/typebox@0.25.13/src/typebox.js":
/*!****************************************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/deps/deno.land/x/typebox@0.25.13/src/typebox.js ***!
  \****************************************************************************************/
/***/ ((__unused_webpack___webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "Hint": () => (/* binding */ Hint),
/* harmony export */   "Kind": () => (/* binding */ Kind),
/* harmony export */   "Modifier": () => (/* binding */ Modifier),
/* harmony export */   "Type": () => (/* binding */ Type),
/* harmony export */   "TypeBuilder": () => (/* binding */ TypeBuilder)
/* harmony export */ });
/*--------------------------------------------------------------------------

@sinclair/typebox

The MIT License (MIT)

Copyright (c) 2022 Haydn Paterson (sinclair) <haydn.developer@gmail.com>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

---------------------------------------------------------------------------*/
// --------------------------------------------------------------------------
// Symbols
// --------------------------------------------------------------------------
const Kind = Symbol.for('TypeBox.Kind');
const Hint = Symbol.for('TypeBox.Hint');
const Modifier = Symbol.for('TypeBox.Modifier');
// --------------------------------------------------------------------------
// TypeBuilder
// --------------------------------------------------------------------------
let TypeOrdinal = 0;
class TypeBuilder {
    // ----------------------------------------------------------------------
    // Modifiers
    // ----------------------------------------------------------------------
    /** Creates a readonly optional property */
    ReadonlyOptional(item) {
        return { [Modifier]: 'ReadonlyOptional', ...item };
    }
    /** Creates a readonly property */
    Readonly(item) {
        return { [Modifier]: 'Readonly', ...item };
    }
    /** Creates a optional property */
    Optional(item) {
        return { [Modifier]: 'Optional', ...item };
    }
    // ----------------------------------------------------------------------
    // Types
    // ----------------------------------------------------------------------
    /** `Standard` Creates a any type */
    Any(options = {}) {
        return this.Create({ ...options, [Kind]: 'Any' });
    }
    /** `Standard` Creates a array type */
    Array(items, options = {}) {
        return this.Create({ ...options, [Kind]: 'Array', type: 'array', items });
    }
    /** `Standard` Creates a boolean type */
    Boolean(options = {}) {
        return this.Create({ ...options, [Kind]: 'Boolean', type: 'boolean' });
    }
    /** `Extended` Creates a tuple type from this constructors parameters */
    ConstructorParameters(schema, options = {}) {
        return this.Tuple([...schema.parameters], { ...options });
    }
    /** `Extended` Creates a constructor type */
    Constructor(parameters, returns, options = {}) {
        if (parameters[Kind] === 'Tuple') {
            const inner = parameters.items === undefined ? [] : parameters.items;
            return this.Create({ ...options, [Kind]: 'Constructor', type: 'object', instanceOf: 'Constructor', parameters: inner, returns });
        }
        else if (globalThis.Array.isArray(parameters)) {
            return this.Create({ ...options, [Kind]: 'Constructor', type: 'object', instanceOf: 'Constructor', parameters, returns });
        }
        else {
            throw new Error('TypeBuilder.Constructor: Invalid parameters');
        }
    }
    /** `Extended` Creates a Date type */
    Date(options = {}) {
        return this.Create({ ...options, [Kind]: 'Date', type: 'object', instanceOf: 'Date' });
    }
    /** `Standard` Creates a enum type */
    Enum(item, options = {}) {
        const values = Object.keys(item)
            .filter((key) => isNaN(key))
            .map((key) => item[key]);
        const anyOf = values.map((value) => (typeof value === 'string' ? { [Kind]: 'Literal', type: 'string', const: value } : { [Kind]: 'Literal', type: 'number', const: value }));
        return this.Create({ ...options, [Kind]: 'Union', [Hint]: 'Enum', anyOf });
    }
    /** `Extended` Creates a function type */
    Function(parameters, returns, options = {}) {
        if (parameters[Kind] === 'Tuple') {
            const inner = parameters.items === undefined ? [] : parameters.items;
            return this.Create({ ...options, [Kind]: 'Function', type: 'object', instanceOf: 'Function', parameters: inner, returns });
        }
        else if (globalThis.Array.isArray(parameters)) {
            return this.Create({ ...options, [Kind]: 'Function', type: 'object', instanceOf: 'Function', parameters, returns });
        }
        else {
            throw new Error('TypeBuilder.Function: Invalid parameters');
        }
    }
    /** `Extended` Creates a type from this constructors instance type */
    InstanceType(schema, options = {}) {
        return { ...options, ...this.Clone(schema.returns) };
    }
    /** `Standard` Creates a integer type */
    Integer(options = {}) {
        return this.Create({ ...options, [Kind]: 'Integer', type: 'integer' });
    }
    /** `Standard` Creates a intersect type. */
    Intersect(objects, options = {}) {
        const isOptional = (schema) => (schema[Modifier] && schema[Modifier] === 'Optional') || schema[Modifier] === 'ReadonlyOptional';
        const [required, optional] = [new Set(), new Set()];
        for (const object of objects) {
            for (const [key, schema] of Object.entries(object.properties)) {
                if (isOptional(schema))
                    optional.add(key);
            }
        }
        for (const object of objects) {
            for (const key of Object.keys(object.properties)) {
                if (!optional.has(key))
                    required.add(key);
            }
        }
        const properties = {};
        for (const object of objects) {
            for (const [key, schema] of Object.entries(object.properties)) {
                properties[key] = properties[key] === undefined ? schema : { [Kind]: 'Union', anyOf: [properties[key], { ...schema }] };
            }
        }
        if (required.size > 0) {
            return this.Create({ ...options, [Kind]: 'Object', type: 'object', properties, required: [...required] });
        }
        else {
            return this.Create({ ...options, [Kind]: 'Object', type: 'object', properties });
        }
    }
    /** `Standard` Creates a keyof type */
    KeyOf(object, options = {}) {
        const items = Object.keys(object.properties).map((key) => this.Create({ ...options, [Kind]: 'Literal', type: 'string', const: key }));
        return this.Create({ ...options, [Kind]: 'Union', [Hint]: 'KeyOf', anyOf: items });
    }
    /** `Standard` Creates a literal type. */
    Literal(value, options = {}) {
        return this.Create({ ...options, [Kind]: 'Literal', const: value, type: typeof value });
    }
    /** `Standard` Creates a never type */
    Never(options = {}) {
        return this.Create({
            ...options,
            [Kind]: 'Never',
            allOf: [
                { type: 'boolean', const: false },
                { type: 'boolean', const: true },
            ],
        });
    }
    /** `Standard` Creates a null type */
    Null(options = {}) {
        return this.Create({ ...options, [Kind]: 'Null', type: 'null' });
    }
    /** `Standard` Creates a number type */
    Number(options = {}) {
        return this.Create({ ...options, [Kind]: 'Number', type: 'number' });
    }
    /** `Standard` Creates an object type */
    Object(properties, options = {}) {
        const property_names = Object.keys(properties);
        const optional = property_names.filter((name) => {
            const property = properties[name];
            const modifier = property[Modifier];
            return modifier && (modifier === 'Optional' || modifier === 'ReadonlyOptional');
        });
        const required = property_names.filter((name) => !optional.includes(name));
        if (required.length > 0) {
            return this.Create({ ...options, [Kind]: 'Object', type: 'object', properties, required });
        }
        else {
            return this.Create({ ...options, [Kind]: 'Object', type: 'object', properties });
        }
    }
    /** `Standard` Creates a new object type whose keys are omitted from the given source type */
    Omit(schema, keys, options = {}) {
        const select = keys[Kind] === 'Union' ? keys.anyOf.map((schema) => schema.const) : keys;
        const next = { ...this.Clone(schema), ...options, [Hint]: 'Omit' };
        if (next.required) {
            next.required = next.required.filter((key) => !select.includes(key));
            if (next.required.length === 0)
                delete next.required;
        }
        for (const key of Object.keys(next.properties)) {
            if (select.includes(key))
                delete next.properties[key];
        }
        return this.Create(next);
    }
    /** `Extended` Creates a tuple type from this functions parameters */
    Parameters(schema, options = {}) {
        return Type.Tuple(schema.parameters, { ...options });
    }
    /** `Standard` Creates an object type whose properties are all optional */
    Partial(schema, options = {}) {
        const next = { ...this.Clone(schema), ...options, [Hint]: 'Partial' };
        delete next.required;
        for (const key of Object.keys(next.properties)) {
            const property = next.properties[key];
            const modifer = property[Modifier];
            switch (modifer) {
                case 'ReadonlyOptional':
                    property[Modifier] = 'ReadonlyOptional';
                    break;
                case 'Readonly':
                    property[Modifier] = 'ReadonlyOptional';
                    break;
                case 'Optional':
                    property[Modifier] = 'Optional';
                    break;
                default:
                    property[Modifier] = 'Optional';
                    break;
            }
        }
        return this.Create(next);
    }
    /** `Standard` Creates a new object type whose keys are picked from the given source type */
    Pick(schema, keys, options = {}) {
        const select = keys[Kind] === 'Union' ? keys.anyOf.map((schema) => schema.const) : keys;
        const next = { ...this.Clone(schema), ...options, [Hint]: 'Pick' };
        if (next.required) {
            next.required = next.required.filter((key) => select.includes(key));
            if (next.required.length === 0)
                delete next.required;
        }
        for (const key of Object.keys(next.properties)) {
            if (!select.includes(key))
                delete next.properties[key];
        }
        return this.Create(next);
    }
    /** `Extended` Creates a Promise type */
    Promise(item, options = {}) {
        return this.Create({ ...options, [Kind]: 'Promise', type: 'object', instanceOf: 'Promise', item });
    }
    /** `Standard` Creates a record type */
    Record(key, value, options = {}) {
        // If string literal union return TObject with properties extracted from union.
        if (key[Kind] === 'Union') {
            return this.Object(key.anyOf.reduce((acc, literal) => {
                return { ...acc, [literal.const]: value };
            }, {}), { ...options, [Hint]: 'Record' });
        }
        // otherwise return TRecord with patternProperties
        const pattern = ['Integer', 'Number'].includes(key[Kind]) ? '^(0|[1-9][0-9]*)$' : key[Kind] === 'String' && key.pattern ? key.pattern : '^.*$';
        return this.Create({
            ...options,
            [Kind]: 'Record',
            type: 'object',
            patternProperties: { [pattern]: value },
            additionalProperties: false,
        });
    }
    /** `Standard` Creates recursive type */
    Recursive(callback, options = {}) {
        if (options.$id === undefined)
            options.$id = `T${TypeOrdinal++}`;
        const self = callback({ [Kind]: 'Self', $ref: `${options.$id}` });
        self.$id = options.$id;
        return this.Create({ ...options, ...self });
    }
    /** `Standard` Creates a reference type. The referenced type must contain a $id. */
    Ref(schema, options = {}) {
        if (schema.$id === undefined)
            throw Error('TypeBuilder.Ref: Referenced schema must specify an $id');
        return this.Create({ ...options, [Kind]: 'Ref', $ref: schema.$id });
    }
    /** `Standard` Creates a string type from a regular expression */
    RegEx(regex, options = {}) {
        return this.Create({ ...options, [Kind]: 'String', type: 'string', pattern: regex.source });
    }
    /** `Standard` Creates an object type whose properties are all required */
    Required(schema, options = {}) {
        const next = { ...this.Clone(schema), ...options, [Hint]: 'Required' };
        next.required = Object.keys(next.properties);
        for (const key of Object.keys(next.properties)) {
            const property = next.properties[key];
            const modifier = property[Modifier];
            switch (modifier) {
                case 'ReadonlyOptional':
                    property[Modifier] = 'Readonly';
                    break;
                case 'Readonly':
                    property[Modifier] = 'Readonly';
                    break;
                case 'Optional':
                    delete property[Modifier];
                    break;
                default:
                    delete property[Modifier];
                    break;
            }
        }
        return this.Create(next);
    }
    /** `Extended` Creates a type from this functions return type */
    ReturnType(schema, options = {}) {
        return { ...options, ...this.Clone(schema.returns) };
    }
    /** Removes Kind and Modifier symbol property keys from this schema */
    Strict(schema) {
        return JSON.parse(JSON.stringify(schema));
    }
    /** `Standard` Creates a string type */
    String(options = {}) {
        return this.Create({ ...options, [Kind]: 'String', type: 'string' });
    }
    /** `Standard` Creates a tuple type */
    Tuple(items, options = {}) {
        const additionalItems = false;
        const minItems = items.length;
        const maxItems = items.length;
        const schema = (items.length > 0 ? { ...options, [Kind]: 'Tuple', type: 'array', items, additionalItems, minItems, maxItems } : { ...options, [Kind]: 'Tuple', type: 'array', minItems, maxItems });
        return this.Create(schema);
    }
    /** `Extended` Creates a undefined type */
    Undefined(options = {}) {
        return this.Create({ ...options, [Kind]: 'Undefined', type: 'null', typeOf: 'Undefined' });
    }
    /** `Standard` Creates a union type */
    Union(items, options = {}) {
        return items.length === 0 ? Type.Never({ ...options }) : this.Create({ ...options, [Kind]: 'Union', anyOf: items });
    }
    /** `Extended` Creates a Uint8Array type */
    Uint8Array(options = {}) {
        return this.Create({ ...options, [Kind]: 'Uint8Array', type: 'object', instanceOf: 'Uint8Array' });
    }
    /** `Standard` Creates an unknown type */
    Unknown(options = {}) {
        return this.Create({ ...options, [Kind]: 'Unknown' });
    }
    /** `Standard` Creates a user defined schema that infers as type T  */
    Unsafe(options = {}) {
        return this.Create({ ...options, [Kind]: options[Kind] || 'Unsafe' });
    }
    /** `Extended` Creates a void type */
    Void(options = {}) {
        return this.Create({ ...options, [Kind]: 'Void', type: 'null', typeOf: 'Void' });
    }
    /** Use this function to return TSchema with static and params omitted */
    Create(schema) {
        return schema;
    }
    /** Clones the given value */
    Clone(value) {
        const isObject = (object) => typeof object === 'object' && object !== null && !Array.isArray(object);
        const isArray = (object) => typeof object === 'object' && object !== null && Array.isArray(object);
        if (isObject(value)) {
            return Object.keys(value).reduce((acc, key) => ({
                ...acc,
                [key]: this.Clone(value[key]),
            }), Object.getOwnPropertySymbols(value).reduce((acc, key) => ({
                ...acc,
                [key]: this.Clone(value[key]),
            }), {}));
        }
        else if (isArray(value)) {
            return value.map((item) => this.Clone(item));
        }
        else {
            return value;
        }
    }
}
/** JSON Schema Type Builder with Static Type Resolution for TypeScript */
const Type = new TypeBuilder();


/***/ }),

/***/ "./node_modules/lucid-cardano/esm/mod.js":
/*!***********************************************!*\
  !*** ./node_modules/lucid-cardano/esm/mod.js ***!
  \***********************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "Blockfrost": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.Blockfrost),
/* harmony export */   "C": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.C),
/* harmony export */   "Constr": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.Constr),
/* harmony export */   "Data": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.Data),
/* harmony export */   "Emulator": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.Emulator),
/* harmony export */   "Kupmios": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.Kupmios),
/* harmony export */   "Lucid": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.Lucid),
/* harmony export */   "M": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.M),
/* harmony export */   "MerkleTree": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.MerkleTree),
/* harmony export */   "PROTOCOL_PARAMETERS_DEFAULT": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.PROTOCOL_PARAMETERS_DEFAULT),
/* harmony export */   "SLOT_CONFIG_NETWORK": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.SLOT_CONFIG_NETWORK),
/* harmony export */   "Tx": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.Tx),
/* harmony export */   "TxComplete": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.TxComplete),
/* harmony export */   "TxSigned": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.TxSigned),
/* harmony export */   "Utils": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.Utils),
/* harmony export */   "applyDoubleCborEncoding": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.applyDoubleCborEncoding),
/* harmony export */   "applyParamsToScript": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.applyParamsToScript),
/* harmony export */   "assetsToValue": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.assetsToValue),
/* harmony export */   "combineHash": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.combineHash),
/* harmony export */   "concat": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.concat),
/* harmony export */   "coreToUtxo": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.coreToUtxo),
/* harmony export */   "createCostModels": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.createCostModels),
/* harmony export */   "datumJsonToCbor": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.datumJsonToCbor),
/* harmony export */   "equals": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.equals),
/* harmony export */   "fromHex": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.fromHex),
/* harmony export */   "fromLabel": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.fromLabel),
/* harmony export */   "fromScriptRef": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.fromScriptRef),
/* harmony export */   "fromText": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.fromText),
/* harmony export */   "fromUnit": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.fromUnit),
/* harmony export */   "generatePrivateKey": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.generatePrivateKey),
/* harmony export */   "generateSeedPhrase": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.generateSeedPhrase),
/* harmony export */   "getAddressDetails": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.getAddressDetails),
/* harmony export */   "nativeScriptFromJson": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.nativeScriptFromJson),
/* harmony export */   "networkToId": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.networkToId),
/* harmony export */   "paymentCredentialOf": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.paymentCredentialOf),
/* harmony export */   "sha256": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.sha256),
/* harmony export */   "slotToBeginUnixTime": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.slotToBeginUnixTime),
/* harmony export */   "stakeCredentialOf": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.stakeCredentialOf),
/* harmony export */   "toHex": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.toHex),
/* harmony export */   "toLabel": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.toLabel),
/* harmony export */   "toPublicKey": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.toPublicKey),
/* harmony export */   "toScriptRef": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.toScriptRef),
/* harmony export */   "toText": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.toText),
/* harmony export */   "toUnit": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.toUnit),
/* harmony export */   "unixTimeToEnclosingSlot": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.unixTimeToEnclosingSlot),
/* harmony export */   "utxoToCore": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.utxoToCore),
/* harmony export */   "valueToAssets": () => (/* reexport safe */ _src_mod_js__WEBPACK_IMPORTED_MODULE_0__.valueToAssets)
/* harmony export */ });
/* harmony import */ var _src_mod_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./src/mod.js */ "./node_modules/lucid-cardano/esm/src/mod.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_src_mod_js__WEBPACK_IMPORTED_MODULE_0__]);
_src_mod_js__WEBPACK_IMPORTED_MODULE_0__ = (__webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__)[0];


__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/package.js":
/*!***************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/package.js ***!
  \***************************************************/
/***/ ((__unused_webpack___webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "default": () => (__WEBPACK_DEFAULT_EXPORT__)
/* harmony export */ });
/* harmony default export */ const __WEBPACK_DEFAULT_EXPORT__ = ({
    "name": "lucid-cardano",
    "version": "0.9.4",
    "license": "MIT",
    "author": "Alessandro Konrad",
    "description": "Lucid is a library, which allows you to create Cardano transactions and off-chain code for your Plutus contracts in JavaScript, Deno and Node.js.",
    "repository": "https://github.com/spacebudz/lucid"
});


/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/core/core.js":
/*!*********************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/core/core.js ***!
  \*********************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "C": () => (/* binding */ C),
/* harmony export */   "M": () => (/* binding */ M)
/* harmony export */ });
// dnt-shim-ignore
const isNode = typeof window === "undefined";
if (isNode) {
    const fetch = await import(/* webpackIgnore: true */ "node-fetch");
    const { Crypto } = await import(
    /* webpackIgnore: true */ "@peculiar/webcrypto");
    const { WebSocket } = await import(
    /* webpackIgnore: true */ "ws");
    // @ts-ignore : global
    if (!global.WebSocket)
        global.WebSocket = WebSocket;
    // @ts-ignore : global
    if (!global.crypto)
        global.crypto = new Crypto();
    // @ts-ignore : global
    if (!global.fetch)
        global.fetch = fetch.default;
    // @ts-ignore : global
    if (!global.Headers)
        global.Headers = fetch.Headers;
    // @ts-ignore : global
    if (!global.Request)
        global.Request = fetch.Request;
    // @ts-ignore : global
    if (!global.Response)
        global.Response = fetch.Response;
}
async function importForEnvironmentCore() {
    try {
        if (isNode) {
            return (await import(
            /* webpackIgnore: true */
            "./wasm_modules/cardano_multiplatform_lib_nodejs/cardano_multiplatform_lib.js"));
        }
        const pkg = await __webpack_require__.e(/*! import() */ "vendors-node_modules_lucid-cardano_esm_src_core_wasm_modules_cardano_multiplatform_lib_web_ca-d40d81").then(__webpack_require__.bind(__webpack_require__, /*! ./wasm_modules/cardano_multiplatform_lib_web/cardano_multiplatform_lib.js */ "./node_modules/lucid-cardano/esm/src/core/wasm_modules/cardano_multiplatform_lib_web/cardano_multiplatform_lib.js"));
        await pkg.default(await fetch(new URL(/* asset import */ __webpack_require__(/*! ./wasm_modules/cardano_multiplatform_lib_web/cardano_multiplatform_lib_bg.wasm */ "./node_modules/lucid-cardano/esm/src/core/wasm_modules/cardano_multiplatform_lib_web/cardano_multiplatform_lib_bg.wasm"), __webpack_require__.b)));
        return pkg;
    }
    catch (_e) {
        // This only ever happens during SSR rendering
        return null;
    }
}
async function importForEnvironmentMessage() {
    try {
        if (isNode) {
            return (await import(
            /* webpackIgnore: true */
            "./wasm_modules/cardano_message_signing_nodejs/cardano_message_signing.js"));
        }
        const pkg = await __webpack_require__.e(/*! import() */ "vendors-node_modules_lucid-cardano_esm_src_core_wasm_modules_cardano_message_signing_web_card-b04946").then(__webpack_require__.bind(__webpack_require__, /*! ./wasm_modules/cardano_message_signing_web/cardano_message_signing.js */ "./node_modules/lucid-cardano/esm/src/core/wasm_modules/cardano_message_signing_web/cardano_message_signing.js"));
        await pkg.default(await fetch(new URL(/* asset import */ __webpack_require__(/*! ./wasm_modules/cardano_message_signing_web/cardano_message_signing_bg.wasm */ "./node_modules/lucid-cardano/esm/src/core/wasm_modules/cardano_message_signing_web/cardano_message_signing_bg.wasm"), __webpack_require__.b)));
        return pkg;
    }
    catch (_e) {
        // This only ever happens during SSR rendering
        return null;
    }
}
const [resolvedCore, resolvedMessage] = await Promise.all([
    importForEnvironmentCore(),
    importForEnvironmentMessage(),
]);
const C = resolvedCore;
const M = resolvedMessage;

__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } }, 1);

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/core/mod.js":
/*!********************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/core/mod.js ***!
  \********************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "C": () => (/* reexport safe */ _core_js__WEBPACK_IMPORTED_MODULE_0__.C),
/* harmony export */   "M": () => (/* reexport safe */ _core_js__WEBPACK_IMPORTED_MODULE_0__.M)
/* harmony export */ });
/* harmony import */ var _core_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./core.js */ "./node_modules/lucid-cardano/esm/src/core/core.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_core_js__WEBPACK_IMPORTED_MODULE_0__]);
_core_js__WEBPACK_IMPORTED_MODULE_0__ = (__webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__)[0];


__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/lucid/lucid.js":
/*!***********************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/lucid/lucid.js ***!
  \***********************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "Lucid": () => (/* binding */ Lucid)
/* harmony export */ });
/* harmony import */ var _core_mod_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../core/mod.js */ "./node_modules/lucid-cardano/esm/src/core/mod.js");
/* harmony import */ var _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../utils/mod.js */ "./node_modules/lucid-cardano/esm/src/utils/mod.js");
/* harmony import */ var _tx_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./tx.js */ "./node_modules/lucid-cardano/esm/src/lucid/tx.js");
/* harmony import */ var _tx_complete_js__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./tx_complete.js */ "./node_modules/lucid-cardano/esm/src/lucid/tx_complete.js");
/* harmony import */ var _misc_wallet_js__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../misc/wallet.js */ "./node_modules/lucid-cardano/esm/src/misc/wallet.js");
/* harmony import */ var _misc_sign_data_js__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../misc/sign_data.js */ "./node_modules/lucid-cardano/esm/src/misc/sign_data.js");
/* harmony import */ var _message_js__WEBPACK_IMPORTED_MODULE_6__ = __webpack_require__(/*! ./message.js */ "./node_modules/lucid-cardano/esm/src/lucid/message.js");
/* harmony import */ var _plutus_time_js__WEBPACK_IMPORTED_MODULE_7__ = __webpack_require__(/*! ../plutus/time.js */ "./node_modules/lucid-cardano/esm/src/plutus/time.js");
/* harmony import */ var _plutus_data_js__WEBPACK_IMPORTED_MODULE_8__ = __webpack_require__(/*! ../plutus/data.js */ "./node_modules/lucid-cardano/esm/src/plutus/data.js");
/* harmony import */ var _provider_emulator_js__WEBPACK_IMPORTED_MODULE_9__ = __webpack_require__(/*! ../provider/emulator.js */ "./node_modules/lucid-cardano/esm/src/provider/emulator.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_core_mod_js__WEBPACK_IMPORTED_MODULE_0__, _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__, _tx_js__WEBPACK_IMPORTED_MODULE_2__, _tx_complete_js__WEBPACK_IMPORTED_MODULE_3__, _misc_wallet_js__WEBPACK_IMPORTED_MODULE_4__, _misc_sign_data_js__WEBPACK_IMPORTED_MODULE_5__, _message_js__WEBPACK_IMPORTED_MODULE_6__, _plutus_data_js__WEBPACK_IMPORTED_MODULE_8__, _provider_emulator_js__WEBPACK_IMPORTED_MODULE_9__]);
([_core_mod_js__WEBPACK_IMPORTED_MODULE_0__, _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__, _tx_js__WEBPACK_IMPORTED_MODULE_2__, _tx_complete_js__WEBPACK_IMPORTED_MODULE_3__, _misc_wallet_js__WEBPACK_IMPORTED_MODULE_4__, _misc_sign_data_js__WEBPACK_IMPORTED_MODULE_5__, _message_js__WEBPACK_IMPORTED_MODULE_6__, _plutus_data_js__WEBPACK_IMPORTED_MODULE_8__, _provider_emulator_js__WEBPACK_IMPORTED_MODULE_9__] = __webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__);










class Lucid {
    constructor() {
        Object.defineProperty(this, "txBuilderConfig", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "wallet", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "provider", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "network", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: "Mainnet"
        });
        Object.defineProperty(this, "utils", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
    }
    static async new(provider, network) {
        const lucid = new this();
        if (network)
            lucid.network = network;
        if (provider) {
            lucid.provider = provider;
            const protocolParameters = await provider.getProtocolParameters();
            if (lucid.provider instanceof _provider_emulator_js__WEBPACK_IMPORTED_MODULE_9__.Emulator) {
                lucid.network = "Custom";
                _plutus_time_js__WEBPACK_IMPORTED_MODULE_7__.SLOT_CONFIG_NETWORK[lucid.network] = {
                    zeroTime: lucid.provider.now(),
                    zeroSlot: 0,
                    slotLength: 1000,
                };
            }
            const slotConfig = _plutus_time_js__WEBPACK_IMPORTED_MODULE_7__.SLOT_CONFIG_NETWORK[lucid.network];
            lucid.txBuilderConfig = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionBuilderConfigBuilder["new"]()
                .coins_per_utxo_byte(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(protocolParameters.coinsPerUtxoByte.toString()))
                .fee_algo(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.LinearFee["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(protocolParameters.minFeeA.toString()), _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(protocolParameters.minFeeB.toString())))
                .key_deposit(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(protocolParameters.keyDeposit.toString()))
                .pool_deposit(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(protocolParameters.poolDeposit.toString()))
                .max_tx_size(protocolParameters.maxTxSize)
                .max_value_size(protocolParameters.maxValSize)
                .collateral_percentage(protocolParameters.collateralPercentage)
                .max_collateral_inputs(protocolParameters.maxCollateralInputs)
                .max_tx_ex_units(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.ExUnits["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(protocolParameters.maxTxExMem.toString()), _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(protocolParameters.maxTxExSteps.toString())))
                .ex_unit_prices(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.ExUnitPrices.from_float(protocolParameters.priceMem, protocolParameters.priceStep))
                .slot_config(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(slotConfig.zeroTime.toString()), _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(slotConfig.zeroSlot.toString()), slotConfig.slotLength)
                .blockfrost(
            // We have Aiken now as native plutus core engine (primary), but we still support blockfrost (secondary) in case of bugs.
            _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Blockfrost["new"](
            // deno-lint-ignore no-explicit-any
            (provider?.url || "") + "/utils/txs/evaluate", 
            // deno-lint-ignore no-explicit-any
            provider?.projectId || ""))
                .costmdls((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.createCostModels)(protocolParameters.costModels))
                .build();
        }
        lucid.utils = new _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.Utils(lucid);
        return lucid;
    }
    /**
     * Switch provider and/or network.
     * If provider or network unset, no overwriting happens. Provider or network from current instance are taken then.
     */
    async switchProvider(provider, network) {
        if (this.network === "Custom") {
            throw new Error("Cannot switch when on custom network.");
        }
        const lucid = await Lucid.new(provider, network);
        this.txBuilderConfig = lucid.txBuilderConfig;
        this.provider = provider || this.provider;
        this.network = network || this.network;
        this.wallet = lucid.wallet;
        return this;
    }
    newTx() {
        return new _tx_js__WEBPACK_IMPORTED_MODULE_2__.Tx(this);
    }
    fromTx(tx) {
        return new _tx_complete_js__WEBPACK_IMPORTED_MODULE_3__.TxComplete(this, _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Transaction.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(tx)));
    }
    /** Signs a message. Expects the payload to be Hex encoded. */
    newMessage(address, payload) {
        return new _message_js__WEBPACK_IMPORTED_MODULE_6__.Message(this, address, payload);
    }
    /** Verify a message. Expects the payload to be Hex encoded. */
    verifyMessage(address, payload, signedMessage) {
        const { paymentCredential, stakeCredential, address: { hex: addressHex } } = this.utils.getAddressDetails(address);
        const keyHash = paymentCredential?.hash || stakeCredential?.hash;
        if (!keyHash)
            throw new Error("Not a valid address provided.");
        return (0,_misc_sign_data_js__WEBPACK_IMPORTED_MODULE_5__.verifyData)(addressHex, keyHash, payload, signedMessage);
    }
    currentSlot() {
        return this.utils.unixTimeToSlot(Date.now());
    }
    utxosAt(addressOrCredential) {
        return this.provider.getUtxos(addressOrCredential);
    }
    utxosAtWithUnit(addressOrCredential, unit) {
        return this.provider.getUtxosWithUnit(addressOrCredential, unit);
    }
    /** Unit needs to be an NFT (or optionally the entire supply in one UTxO). */
    utxoByUnit(unit) {
        return this.provider.getUtxoByUnit(unit);
    }
    utxosByOutRef(outRefs) {
        return this.provider.getUtxosByOutRef(outRefs);
    }
    delegationAt(rewardAddress) {
        return this.provider.getDelegation(rewardAddress);
    }
    awaitTx(txHash, checkInterval = 3000) {
        return this.provider.awaitTx(txHash, checkInterval);
    }
    async datumOf(utxo, shape) {
        if (!utxo.datum) {
            if (!utxo.datumHash) {
                throw new Error("This UTxO does not have a datum hash.");
            }
            utxo.datum = await this.provider.getDatum(utxo.datumHash);
        }
        return shape ? _plutus_data_js__WEBPACK_IMPORTED_MODULE_8__.Data.from(utxo.datum, shape) : utxo.datum;
    }
    /**
     * Cardano Private key in bech32; not the BIP32 private key or any key that is not fully derived.
     * Only an Enteprise address (without stake credential) is derived.
     */
    selectWalletFromPrivateKey(privateKey) {
        const priv = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PrivateKey.from_bech32(privateKey);
        const pubKeyHash = priv.to_public().hash();
        this.wallet = {
            // deno-lint-ignore require-await
            address: async () => _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.EnterpriseAddress["new"](this.network === "Mainnet" ? 1 : 0, _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.StakeCredential.from_keyhash(pubKeyHash))
                .to_address()
                .to_bech32(undefined),
            // deno-lint-ignore require-await
            rewardAddress: async () => null,
            getUtxos: async () => {
                return await this.utxosAt((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.paymentCredentialOf)(await this.wallet.address()));
            },
            getUtxosCore: async () => {
                const utxos = await this.utxosAt((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.paymentCredentialOf)(await this.wallet.address()));
                const coreUtxos = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionUnspentOutputs["new"]();
                utxos.forEach((utxo) => {
                    coreUtxos.add((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.utxoToCore)(utxo));
                });
                return coreUtxos;
            },
            // deno-lint-ignore require-await
            getDelegation: async () => {
                return { poolId: null, rewards: 0n };
            },
            // deno-lint-ignore require-await
            signTx: async (tx) => {
                const witness = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.make_vkey_witness(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.hash_transaction(tx.body()), priv);
                const txWitnessSetBuilder = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionWitnessSetBuilder["new"]();
                txWitnessSetBuilder.add_vkey(witness);
                return txWitnessSetBuilder.build();
            },
            // deno-lint-ignore require-await
            signMessage: async (address, payload) => {
                const { paymentCredential, address: { hex: hexAddress } } = this.utils
                    .getAddressDetails(address);
                const keyHash = paymentCredential?.hash;
                const originalKeyHash = pubKeyHash.to_hex();
                if (!keyHash || keyHash !== originalKeyHash) {
                    throw new Error(`Cannot sign message for address: ${address}.`);
                }
                return (0,_misc_sign_data_js__WEBPACK_IMPORTED_MODULE_5__.signData)(hexAddress, payload, privateKey);
            },
            submitTx: async (tx) => {
                return await this.provider.submitTx(tx);
            },
        };
        return this;
    }
    selectWallet(api) {
        const getAddressHex = async () => {
            const [addressHex] = await api.getUsedAddresses();
            if (addressHex)
                return addressHex;
            const [unusedAddressHex] = await api.getUnusedAddresses();
            return unusedAddressHex;
        };
        this.wallet = {
            address: async () => _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Address.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(await getAddressHex())).to_bech32(undefined),
            rewardAddress: async () => {
                const [rewardAddressHex] = await api.getRewardAddresses();
                const rewardAddress = rewardAddressHex
                    ? _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.RewardAddress.from_address(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Address.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(rewardAddressHex)))
                        .to_address()
                        .to_bech32(undefined)
                    : null;
                return rewardAddress;
            },
            getUtxos: async () => {
                const utxos = ((await api.getUtxos()) || []).map((utxo) => {
                    const parsedUtxo = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionUnspentOutput.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(utxo));
                    return (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.coreToUtxo)(parsedUtxo);
                });
                return utxos;
            },
            getUtxosCore: async () => {
                const utxos = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionUnspentOutputs["new"]();
                ((await api.getUtxos()) || []).forEach((utxo) => {
                    utxos.add(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionUnspentOutput.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(utxo)));
                });
                return utxos;
            },
            getDelegation: async () => {
                const rewardAddr = await this.wallet.rewardAddress();
                return rewardAddr
                    ? await this.delegationAt(rewardAddr)
                    : { poolId: null, rewards: 0n };
            },
            signTx: async (tx) => {
                const witnessSet = await api.signTx((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.toHex)(tx.to_bytes()), true);
                return _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionWitnessSet.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(witnessSet));
            },
            signMessage: async (address, payload) => {
                const hexAddress = (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.toHex)(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Address.from_bech32(address).to_bytes());
                return await api.signData(hexAddress, payload);
            },
            submitTx: async (tx) => {
                const txHash = await api.submitTx(tx);
                return txHash;
            },
        };
        return this;
    }
    /**
     * Emulates a wallet by constructing it with the utxos and an address.
     * If utxos are not set, utxos are fetched from the provided address.
     */
    selectWalletFrom({ address, utxos, rewardAddress, }) {
        const addressDetails = this.utils.getAddressDetails(address);
        this.wallet = {
            // deno-lint-ignore require-await
            address: async () => address,
            // deno-lint-ignore require-await
            rewardAddress: async () => {
                const rewardAddr = !rewardAddress && addressDetails.stakeCredential
                    ? (() => {
                        if (addressDetails.stakeCredential.type === "Key") {
                            return _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.RewardAddress["new"](this.network === "Mainnet" ? 1 : 0, _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.StakeCredential.from_keyhash(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Ed25519KeyHash.from_hex(addressDetails.stakeCredential.hash)))
                                .to_address()
                                .to_bech32(undefined);
                        }
                        return _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.RewardAddress["new"](this.network === "Mainnet" ? 1 : 0, _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.StakeCredential.from_scripthash(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptHash.from_hex(addressDetails.stakeCredential.hash)))
                            .to_address()
                            .to_bech32(undefined);
                    })()
                    : rewardAddress;
                return rewardAddr || null;
            },
            getUtxos: async () => {
                return utxos ? utxos : await this.utxosAt((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.paymentCredentialOf)(address));
            },
            getUtxosCore: async () => {
                const coreUtxos = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionUnspentOutputs["new"]();
                (utxos ? utxos : await this.utxosAt((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.paymentCredentialOf)(address)))
                    .forEach((utxo) => coreUtxos.add((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.utxoToCore)(utxo)));
                return coreUtxos;
            },
            getDelegation: async () => {
                const rewardAddr = await this.wallet.rewardAddress();
                return rewardAddr
                    ? await this.delegationAt(rewardAddr)
                    : { poolId: null, rewards: 0n };
            },
            // deno-lint-ignore require-await
            signTx: async () => {
                throw new Error("Not implemented");
            },
            // deno-lint-ignore require-await
            signMessage: async () => {
                throw new Error("Not implemented");
            },
            submitTx: async (tx) => {
                return await this.provider.submitTx(tx);
            },
        };
        return this;
    }
    /**
     * Select wallet from a seed phrase (e.g. 15 or 24 words). You have the option to choose between a Base address (with stake credential)
     * and Enterprise address (without stake credential). You can also decide which account index to derive. By default account 0 is derived.
     */
    selectWalletFromSeed(seed, options) {
        const { address, rewardAddress, paymentKey, stakeKey } = (0,_misc_wallet_js__WEBPACK_IMPORTED_MODULE_4__.walletFromSeed)(seed, {
            addressType: options?.addressType || "Base",
            accountIndex: options?.accountIndex || 0,
            password: options?.password,
            network: this.network,
        });
        const paymentKeyHash = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PrivateKey.from_bech32(paymentKey).to_public()
            .hash().to_hex();
        const stakeKeyHash = stakeKey
            ? _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PrivateKey.from_bech32(stakeKey).to_public().hash().to_hex()
            : "";
        const privKeyHashMap = {
            [paymentKeyHash]: paymentKey,
            [stakeKeyHash]: stakeKey,
        };
        this.wallet = {
            // deno-lint-ignore require-await
            address: async () => address,
            // deno-lint-ignore require-await
            rewardAddress: async () => rewardAddress || null,
            // deno-lint-ignore require-await
            getUtxos: async () => this.utxosAt((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.paymentCredentialOf)(address)),
            getUtxosCore: async () => {
                const coreUtxos = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionUnspentOutputs["new"]();
                (await this.utxosAt((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.paymentCredentialOf)(address))).forEach((utxo) => coreUtxos.add((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.utxoToCore)(utxo)));
                return coreUtxos;
            },
            getDelegation: async () => {
                const rewardAddr = await this.wallet.rewardAddress();
                return rewardAddr
                    ? await this.delegationAt(rewardAddr)
                    : { poolId: null, rewards: 0n };
            },
            signTx: async (tx) => {
                const utxos = await this.utxosAt(address);
                const ownKeyHashes = [paymentKeyHash, stakeKeyHash];
                const usedKeyHashes = (0,_misc_wallet_js__WEBPACK_IMPORTED_MODULE_4__.discoverOwnUsedTxKeyHashes)(tx, ownKeyHashes, utxos);
                const txWitnessSetBuilder = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionWitnessSetBuilder["new"]();
                usedKeyHashes.forEach((keyHash) => {
                    const witness = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.make_vkey_witness(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.hash_transaction(tx.body()), _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PrivateKey.from_bech32(privKeyHashMap[keyHash]));
                    txWitnessSetBuilder.add_vkey(witness);
                });
                return txWitnessSetBuilder.build();
            },
            // deno-lint-ignore require-await
            signMessage: async (address, payload) => {
                const { paymentCredential, stakeCredential, address: { hex: hexAddress }, } = this.utils
                    .getAddressDetails(address);
                const keyHash = paymentCredential?.hash || stakeCredential?.hash;
                const privateKey = privKeyHashMap[keyHash];
                if (!privateKey) {
                    throw new Error(`Cannot sign message for address: ${address}.`);
                }
                return (0,_misc_sign_data_js__WEBPACK_IMPORTED_MODULE_5__.signData)(hexAddress, payload, privateKey);
            },
            submitTx: async (tx) => {
                return await this.provider.submitTx(tx);
            },
        };
        return this;
    }
}

__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/lucid/message.js":
/*!*************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/lucid/message.js ***!
  \*************************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "Message": () => (/* binding */ Message)
/* harmony export */ });
/* harmony import */ var _misc_sign_data_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../misc/sign_data.js */ "./node_modules/lucid-cardano/esm/src/misc/sign_data.js");
/* harmony import */ var _mod_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../mod.js */ "./node_modules/lucid-cardano/esm/src/mod.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_misc_sign_data_js__WEBPACK_IMPORTED_MODULE_0__, _mod_js__WEBPACK_IMPORTED_MODULE_1__]);
([_misc_sign_data_js__WEBPACK_IMPORTED_MODULE_0__, _mod_js__WEBPACK_IMPORTED_MODULE_1__] = __webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__);


class Message {
    constructor(lucid, address, payload) {
        Object.defineProperty(this, "lucid", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "address", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "payload", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        this.lucid = lucid;
        this.address = address;
        this.payload = payload;
    }
    /** Sign message with selected wallet. */
    sign() {
        return this.lucid.wallet.signMessage(this.address, this.payload);
    }
    /** Sign message with a separate private key. */
    signWithPrivateKey(privateKey) {
        const { paymentCredential, stakeCredential, address: { hex: hexAddress } } = this.lucid.utils.getAddressDetails(this.address);
        const keyHash = paymentCredential?.hash || stakeCredential?.hash;
        const keyHashOriginal = _mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PrivateKey.from_bech32(privateKey).to_public()
            .hash().to_hex();
        if (!keyHash || keyHash !== keyHashOriginal) {
            throw new Error(`Cannot sign message for address: ${this.address}.`);
        }
        return (0,_misc_sign_data_js__WEBPACK_IMPORTED_MODULE_0__.signData)(hexAddress, this.payload, privateKey);
    }
}

__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/lucid/mod.js":
/*!*********************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/lucid/mod.js ***!
  \*********************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "Lucid": () => (/* reexport safe */ _lucid_js__WEBPACK_IMPORTED_MODULE_0__.Lucid),
/* harmony export */   "Tx": () => (/* reexport safe */ _tx_js__WEBPACK_IMPORTED_MODULE_1__.Tx),
/* harmony export */   "TxComplete": () => (/* reexport safe */ _tx_complete_js__WEBPACK_IMPORTED_MODULE_2__.TxComplete),
/* harmony export */   "TxSigned": () => (/* reexport safe */ _tx_signed_js__WEBPACK_IMPORTED_MODULE_3__.TxSigned)
/* harmony export */ });
/* harmony import */ var _lucid_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./lucid.js */ "./node_modules/lucid-cardano/esm/src/lucid/lucid.js");
/* harmony import */ var _tx_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./tx.js */ "./node_modules/lucid-cardano/esm/src/lucid/tx.js");
/* harmony import */ var _tx_complete_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./tx_complete.js */ "./node_modules/lucid-cardano/esm/src/lucid/tx_complete.js");
/* harmony import */ var _tx_signed_js__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./tx_signed.js */ "./node_modules/lucid-cardano/esm/src/lucid/tx_signed.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_lucid_js__WEBPACK_IMPORTED_MODULE_0__, _tx_js__WEBPACK_IMPORTED_MODULE_1__, _tx_complete_js__WEBPACK_IMPORTED_MODULE_2__, _tx_signed_js__WEBPACK_IMPORTED_MODULE_3__]);
([_lucid_js__WEBPACK_IMPORTED_MODULE_0__, _tx_js__WEBPACK_IMPORTED_MODULE_1__, _tx_complete_js__WEBPACK_IMPORTED_MODULE_2__, _tx_signed_js__WEBPACK_IMPORTED_MODULE_3__] = __webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__);





__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/lucid/tx.js":
/*!********************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/lucid/tx.js ***!
  \********************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "Tx": () => (/* binding */ Tx)
/* harmony export */ });
/* harmony import */ var _core_mod_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../core/mod.js */ "./node_modules/lucid-cardano/esm/src/core/mod.js");
/* harmony import */ var _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../utils/mod.js */ "./node_modules/lucid-cardano/esm/src/utils/mod.js");
/* harmony import */ var _utils_utils_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../utils/utils.js */ "./node_modules/lucid-cardano/esm/src/utils/utils.js");
/* harmony import */ var _tx_complete_js__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./tx_complete.js */ "./node_modules/lucid-cardano/esm/src/lucid/tx_complete.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_core_mod_js__WEBPACK_IMPORTED_MODULE_0__, _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__, _utils_utils_js__WEBPACK_IMPORTED_MODULE_2__, _tx_complete_js__WEBPACK_IMPORTED_MODULE_3__]);
([_core_mod_js__WEBPACK_IMPORTED_MODULE_0__, _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__, _utils_utils_js__WEBPACK_IMPORTED_MODULE_2__, _tx_complete_js__WEBPACK_IMPORTED_MODULE_3__] = __webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__);




class Tx {
    constructor(lucid) {
        Object.defineProperty(this, "txBuilder", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        /** Stores the tx instructions, which get executed after calling .complete() */
        Object.defineProperty(this, "tasks", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "lucid", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        this.lucid = lucid;
        this.txBuilder = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionBuilder["new"](this.lucid.txBuilderConfig);
        this.tasks = [];
    }
    /** Read data from utxos. These utxos are only referenced and not spent. */
    readFrom(utxos) {
        this.tasks.push(async (that) => {
            for (const utxo of utxos) {
                if (utxo.datumHash) {
                    utxo.datum = await that.lucid.datumOf(utxo);
                    // Add datum to witness set, so it can be read from validators
                    const plutusData = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusData.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(utxo.datum));
                    that.txBuilder.add_plutus_data(plutusData);
                }
                const coreUtxo = (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.utxoToCore)(utxo);
                that.txBuilder.add_reference_input(coreUtxo);
            }
        });
        return this;
    }
    /**
     * A public key or native script input.
     * With redeemer it's a plutus script input.
     */
    collectFrom(utxos, redeemer) {
        this.tasks.push(async (that) => {
            for (const utxo of utxos) {
                if (utxo.datumHash && !utxo.datum) {
                    utxo.datum = await that.lucid.datumOf(utxo);
                }
                const coreUtxo = (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.utxoToCore)(utxo);
                that.txBuilder.add_input(coreUtxo, redeemer &&
                    _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptWitness.new_plutus_witness(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusWitness["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusData.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(redeemer)), utxo.datumHash && utxo.datum
                        ? _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusData.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(utxo.datum))
                        : undefined, undefined)));
            }
        });
        return this;
    }
    /**
     * All assets should be of the same policy id.
     * You can chain mintAssets functions together if you need to mint assets with different policy ids.
     * If the plutus script doesn't need a redeemer, you still need to specifiy the void redeemer.
     */
    mintAssets(assets, redeemer) {
        this.tasks.push((that) => {
            const units = Object.keys(assets);
            const policyId = units[0].slice(0, 56);
            const mintAssets = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.MintAssets["new"]();
            units.forEach((unit) => {
                if (unit.slice(0, 56) !== policyId) {
                    throw new Error("Only one policy id allowed. You can chain multiple mintAssets functions together if you need to mint assets with different policy ids.");
                }
                mintAssets.insert(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.AssetName["new"]((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(unit.slice(56))), _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Int.from_str(assets[unit].toString()));
            });
            const scriptHash = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptHash.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(policyId));
            that.txBuilder.add_mint(scriptHash, mintAssets, redeemer
                ? _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptWitness.new_plutus_witness(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusWitness["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusData.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(redeemer)), undefined, undefined))
                : undefined);
        });
        return this;
    }
    /** Pay to a public key or native script address. */
    payToAddress(address, assets) {
        this.tasks.push((that) => {
            const output = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionOutput["new"](addressFromWithNetworkCheck(address, that.lucid), (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.assetsToValue)(assets));
            that.txBuilder.add_output(output);
        });
        return this;
    }
    /** Pay to a public key or native script address with datum or scriptRef. */
    payToAddressWithData(address, outputData, assets) {
        this.tasks.push((that) => {
            if (typeof outputData === "string") {
                outputData = { asHash: outputData };
            }
            if ([outputData.hash, outputData.asHash, outputData.inline].filter((b) => b)
                .length > 1) {
                throw new Error("Not allowed to set hash, asHash and inline at the same time.");
            }
            const output = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionOutput["new"](addressFromWithNetworkCheck(address, that.lucid), (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.assetsToValue)(assets));
            if (outputData.hash) {
                output.set_datum(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Datum.new_data_hash(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.DataHash.from_hex(outputData.hash)));
            }
            else if (outputData.asHash) {
                const plutusData = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusData.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(outputData.asHash));
                output.set_datum(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Datum.new_data_hash(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.hash_plutus_data(plutusData)));
                that.txBuilder.add_plutus_data(plutusData);
            }
            else if (outputData.inline) {
                const plutusData = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusData.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(outputData.inline));
                output.set_datum(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Datum.new_data(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Data["new"](plutusData)));
            }
            const script = outputData.scriptRef;
            if (script) {
                output.set_script_ref((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.toScriptRef)(script));
            }
            that.txBuilder.add_output(output);
        });
        return this;
    }
    /** Pay to a plutus script address with datum or scriptRef. */
    payToContract(address, outputData, assets) {
        if (typeof outputData === "string") {
            outputData = { asHash: outputData };
        }
        if (!(outputData.hash || outputData.asHash || outputData.inline)) {
            throw new Error("No datum set. Script output becomes unspendable without datum.");
        }
        return this.payToAddressWithData(address, outputData, assets);
    }
    /** Delegate to a stake pool. */
    delegateTo(rewardAddress, poolId, redeemer) {
        this.tasks.push((that) => {
            const addressDetails = that.lucid.utils.getAddressDetails(rewardAddress);
            if (addressDetails.type !== "Reward" ||
                !addressDetails.stakeCredential) {
                throw new Error("Not a reward address provided.");
            }
            const credential = addressDetails.stakeCredential.type === "Key"
                ? _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.StakeCredential.from_keyhash(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Ed25519KeyHash.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(addressDetails.stakeCredential.hash)))
                : _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.StakeCredential.from_scripthash(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptHash.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(addressDetails.stakeCredential.hash)));
            that.txBuilder.add_certificate(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Certificate.new_stake_delegation(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.StakeDelegation["new"](credential, _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Ed25519KeyHash.from_bech32(poolId))), redeemer
                ? _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptWitness.new_plutus_witness(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusWitness["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusData.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(redeemer)), undefined, undefined))
                : undefined);
        });
        return this;
    }
    /** Register a reward address in order to delegate to a pool and receive rewards. */
    registerStake(rewardAddress) {
        this.tasks.push((that) => {
            const addressDetails = that.lucid.utils.getAddressDetails(rewardAddress);
            if (addressDetails.type !== "Reward" ||
                !addressDetails.stakeCredential) {
                throw new Error("Not a reward address provided.");
            }
            const credential = addressDetails.stakeCredential.type === "Key"
                ? _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.StakeCredential.from_keyhash(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Ed25519KeyHash.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(addressDetails.stakeCredential.hash)))
                : _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.StakeCredential.from_scripthash(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptHash.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(addressDetails.stakeCredential.hash)));
            that.txBuilder.add_certificate(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Certificate.new_stake_registration(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.StakeRegistration["new"](credential)), undefined);
        });
        return this;
    }
    /** Deregister a reward address. */
    deregisterStake(rewardAddress, redeemer) {
        this.tasks.push((that) => {
            const addressDetails = that.lucid.utils.getAddressDetails(rewardAddress);
            if (addressDetails.type !== "Reward" ||
                !addressDetails.stakeCredential) {
                throw new Error("Not a reward address provided.");
            }
            const credential = addressDetails.stakeCredential.type === "Key"
                ? _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.StakeCredential.from_keyhash(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Ed25519KeyHash.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(addressDetails.stakeCredential.hash)))
                : _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.StakeCredential.from_scripthash(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptHash.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(addressDetails.stakeCredential.hash)));
            that.txBuilder.add_certificate(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Certificate.new_stake_deregistration(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.StakeDeregistration["new"](credential)), redeemer
                ? _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptWitness.new_plutus_witness(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusWitness["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusData.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(redeemer)), undefined, undefined))
                : undefined);
        });
        return this;
    }
    /** Register a stake pool. A pool deposit is required. The metadataUrl needs to be hosted already before making the registration. */
    registerPool(poolParams) {
        this.tasks.push(async (that) => {
            const poolRegistration = await createPoolRegistration(poolParams, that.lucid);
            const certificate = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Certificate.new_pool_registration(poolRegistration);
            that.txBuilder.add_certificate(certificate, undefined);
        });
        return this;
    }
    /** Update a stake pool. No pool deposit is required. The metadataUrl needs to be hosted already before making the update. */
    updatePool(poolParams) {
        this.tasks.push(async (that) => {
            const poolRegistration = await createPoolRegistration(poolParams, that.lucid);
            // This flag makes sure a pool deposit is not required
            poolRegistration.set_is_update(true);
            const certificate = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Certificate.new_pool_registration(poolRegistration);
            that.txBuilder.add_certificate(certificate, undefined);
        });
        return this;
    }
    /**
     * Retire a stake pool. The epoch needs to be the greater than the current epoch + 1 and less than current epoch + eMax.
     * The pool deposit will be sent to reward address as reward after full retirement of the pool.
     */
    retirePool(poolId, epoch) {
        this.tasks.push((that) => {
            const certificate = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Certificate.new_pool_retirement(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PoolRetirement["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Ed25519KeyHash.from_bech32(poolId), epoch));
            that.txBuilder.add_certificate(certificate, undefined);
        });
        return this;
    }
    withdraw(rewardAddress, amount, redeemer) {
        this.tasks.push((that) => {
            that.txBuilder.add_withdrawal(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.RewardAddress.from_address(addressFromWithNetworkCheck(rewardAddress, that.lucid)), _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(amount.toString()), redeemer
                ? _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptWitness.new_plutus_witness(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusWitness["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusData.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(redeemer)), undefined, undefined))
                : undefined);
        });
        return this;
    }
    /**
     * Needs to be a public key address.
     * The PaymentKeyHash is taken when providing a Base, Enterprise or Pointer address.
     * The StakeKeyHash is taken when providing a Reward address.
     */
    addSigner(address) {
        const addressDetails = this.lucid.utils.getAddressDetails(address);
        if (!addressDetails.paymentCredential && !addressDetails.stakeCredential) {
            throw new Error("Not a valid address.");
        }
        const credential = addressDetails.type === "Reward"
            ? addressDetails.stakeCredential
            : addressDetails.paymentCredential;
        if (credential.type === "Script") {
            throw new Error("Only key hashes are allowed as signers.");
        }
        return this.addSignerKey(credential.hash);
    }
    /** Add a payment or stake key hash as a required signer of the transaction. */
    addSignerKey(keyHash) {
        this.tasks.push((that) => {
            that.txBuilder.add_required_signer(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Ed25519KeyHash.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(keyHash)));
        });
        return this;
    }
    validFrom(unixTime) {
        this.tasks.push((that) => {
            const slot = that.lucid.utils.unixTimeToSlot(unixTime);
            that.txBuilder.set_validity_start_interval(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(slot.toString()));
        });
        return this;
    }
    validTo(unixTime) {
        this.tasks.push((that) => {
            const slot = that.lucid.utils.unixTimeToSlot(unixTime);
            that.txBuilder.set_ttl(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(slot.toString()));
        });
        return this;
    }
    attachMetadata(label, metadata) {
        this.tasks.push((that) => {
            that.txBuilder.add_json_metadatum(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(label.toString()), JSON.stringify(metadata));
        });
        return this;
    }
    /** Converts strings to bytes if prefixed with **'0x'**. */
    attachMetadataWithConversion(label, metadata) {
        this.tasks.push((that) => {
            that.txBuilder.add_json_metadatum_with_schema(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(label.toString()), JSON.stringify(metadata), _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.MetadataJsonSchema.BasicConversions);
        });
        return this;
    }
    /** Explicitely set the network id in the transaction body. */
    addNetworkId(id) {
        this.tasks.push((that) => {
            that.txBuilder.set_network_id(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.NetworkId.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(id.toString(16).padStart(2, "0"))));
        });
        return this;
    }
    attachSpendingValidator(spendingValidator) {
        this.tasks.push((that) => {
            attachScript(that, spendingValidator);
        });
        return this;
    }
    attachMintingPolicy(mintingPolicy) {
        this.tasks.push((that) => {
            attachScript(that, mintingPolicy);
        });
        return this;
    }
    attachCertificateValidator(certValidator) {
        this.tasks.push((that) => {
            attachScript(that, certValidator);
        });
        return this;
    }
    attachWithdrawalValidator(withdrawalValidator) {
        this.tasks.push((that) => {
            attachScript(that, withdrawalValidator);
        });
        return this;
    }
    /** Compose transactions. */
    compose(tx) {
        if (tx)
            this.tasks = this.tasks.concat(tx.tasks);
        return this;
    }
    async complete(options) {
        if ([
            options?.change?.outputData?.hash,
            options?.change?.outputData?.asHash,
            options?.change?.outputData?.inline,
        ].filter((b) => b)
            .length > 1) {
            throw new Error("Not allowed to set hash, asHash and inline at the same time.");
        }
        let task = this.tasks.shift();
        while (task) {
            await task(this);
            task = this.tasks.shift();
        }
        const utxos = await this.lucid.wallet.getUtxosCore();
        const changeAddress = addressFromWithNetworkCheck(options?.change?.address || (await this.lucid.wallet.address()), this.lucid);
        if (options?.coinSelection || options?.coinSelection === undefined) {
            this.txBuilder.add_inputs_from(utxos, changeAddress);
        }
        this.txBuilder.balance(changeAddress, (() => {
            if (options?.change?.outputData?.hash) {
                return _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Datum.new_data_hash(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.DataHash.from_hex(options.change.outputData.hash));
            }
            else if (options?.change?.outputData?.asHash) {
                this.txBuilder.add_plutus_data(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusData.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(options.change.outputData.asHash)));
                return _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Datum.new_data_hash(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.hash_plutus_data(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusData.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(options.change.outputData.asHash))));
            }
            else if (options?.change?.outputData?.inline) {
                return _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Datum.new_data(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Data["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusData.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(options.change.outputData.inline))));
            }
            else {
                return undefined;
            }
        })());
        return new _tx_complete_js__WEBPACK_IMPORTED_MODULE_3__.TxComplete(this.lucid, await this.txBuilder.construct(utxos, changeAddress, options?.nativeUplc === undefined ? true : options?.nativeUplc));
    }
    /** Return the current transaction body in Hex encoded Cbor. */
    async toString() {
        let task = this.tasks.shift();
        while (task) {
            await task(this);
            task = this.tasks.shift();
        }
        return (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.toHex)(this.txBuilder.to_bytes());
    }
}
function attachScript(tx, { type, script }) {
    if (type === "Native") {
        return tx.txBuilder.add_native_script(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.NativeScript.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(script)));
    }
    else if (type === "PlutusV1") {
        return tx.txBuilder.add_plutus_script(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusScript.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)((0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.applyDoubleCborEncoding)(script))));
    }
    else if (type === "PlutusV2") {
        return tx.txBuilder.add_plutus_v2_script(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusScript.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)((0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.applyDoubleCborEncoding)(script))));
    }
    throw new Error("No variant matched.");
}
async function createPoolRegistration(poolParams, lucid) {
    const poolOwners = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Ed25519KeyHashes["new"]();
    poolParams.owners.forEach((owner) => {
        const { stakeCredential } = lucid.utils.getAddressDetails(owner);
        if (stakeCredential?.type === "Key") {
            poolOwners.add(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Ed25519KeyHash.from_hex(stakeCredential.hash));
        }
        else
            throw new Error("Only key hashes allowed for pool owners.");
    });
    const metadata = poolParams.metadataUrl
        ? await fetch(poolParams.metadataUrl)
            .then((res) => res.arrayBuffer())
        : null;
    const metadataHash = metadata
        ? _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PoolMetadataHash.from_bytes(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.hash_blake2b256(new Uint8Array(metadata)))
        : null;
    const relays = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Relays["new"]();
    poolParams.relays.forEach((relay) => {
        switch (relay.type) {
            case "SingleHostIp": {
                const ipV4 = relay.ipV4
                    ? _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Ipv4["new"](new Uint8Array(relay.ipV4.split(".").map((b) => parseInt(b))))
                    : undefined;
                const ipV6 = relay.ipV6
                    ? _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Ipv6["new"]((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(relay.ipV6.replaceAll(":", "")))
                    : undefined;
                relays.add(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Relay.new_single_host_addr(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.SingleHostAddr["new"](relay.port, ipV4, ipV6)));
                break;
            }
            case "SingleHostDomainName": {
                relays.add(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Relay.new_single_host_name(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.SingleHostName["new"](relay.port, _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.DNSRecordAorAAAA["new"](relay.domainName))));
                break;
            }
            case "MultiHost": {
                relays.add(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Relay.new_multi_host_name(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.MultiHostName["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.DNSRecordSRV["new"](relay.domainName))));
                break;
            }
        }
    });
    return _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PoolRegistration["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PoolParams["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Ed25519KeyHash.from_bech32(poolParams.poolId), _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.VRFKeyHash.from_hex(poolParams.vrfKeyHash), _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(poolParams.pledge.toString()), _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(poolParams.cost.toString()), _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.UnitInterval.from_float(poolParams.margin), _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.RewardAddress.from_address(addressFromWithNetworkCheck(poolParams.rewardAddress, lucid)), poolOwners, relays, metadataHash
        ? _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PoolMetadata["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.URL["new"](poolParams.metadataUrl), metadataHash)
        : undefined));
}
function addressFromWithNetworkCheck(address, lucid) {
    const addressDetails = lucid.utils.getAddressDetails(address);
    const actualNetworkId = (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.networkToId)(lucid.network);
    if (addressDetails.networkId !== actualNetworkId) {
        throw new Error(`Invalid address: Expected address with network id ${actualNetworkId}, but got ${addressDetails.networkId}`);
    }
    return _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Address.from_bech32(address);
}

__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/lucid/tx_complete.js":
/*!*****************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/lucid/tx_complete.js ***!
  \*****************************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "TxComplete": () => (/* binding */ TxComplete)
/* harmony export */ });
/* harmony import */ var _core_mod_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../core/mod.js */ "./node_modules/lucid-cardano/esm/src/core/mod.js");
/* harmony import */ var _tx_signed_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./tx_signed.js */ "./node_modules/lucid-cardano/esm/src/lucid/tx_signed.js");
/* harmony import */ var _utils_mod_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../utils/mod.js */ "./node_modules/lucid-cardano/esm/src/utils/mod.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_core_mod_js__WEBPACK_IMPORTED_MODULE_0__, _tx_signed_js__WEBPACK_IMPORTED_MODULE_1__, _utils_mod_js__WEBPACK_IMPORTED_MODULE_2__]);
([_core_mod_js__WEBPACK_IMPORTED_MODULE_0__, _tx_signed_js__WEBPACK_IMPORTED_MODULE_1__, _utils_mod_js__WEBPACK_IMPORTED_MODULE_2__] = __webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__);



class TxComplete {
    constructor(lucid, tx) {
        Object.defineProperty(this, "txComplete", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "witnessSetBuilder", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "tasks", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "lucid", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        this.lucid = lucid;
        this.txComplete = tx;
        this.witnessSetBuilder = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionWitnessSetBuilder["new"]();
        this.tasks = [];
    }
    sign() {
        this.tasks.push(async () => {
            const witnesses = await this.lucid.wallet.signTx(this.txComplete);
            this.witnessSetBuilder.add_existing(witnesses);
        });
        return this;
    }
    /** Add an extra signature from a private key. */
    signWithPrivateKey(privateKey) {
        const priv = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PrivateKey.from_bech32(privateKey);
        const witness = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.make_vkey_witness(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.hash_transaction(this.txComplete.body()), priv);
        this.witnessSetBuilder.add_vkey(witness);
        return this;
    }
    /** Sign the transaction and return the witnesses that were just made. */
    async partialSign() {
        const witnesses = await this.lucid.wallet.signTx(this.txComplete);
        this.witnessSetBuilder.add_existing(witnesses);
        return (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_2__.toHex)(witnesses.to_bytes());
    }
    /**
     * Sign the transaction and return the witnesses that were just made.
     * Add an extra signature from a private key.
     */
    partialSignWithPrivateKey(privateKey) {
        const priv = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PrivateKey.from_bech32(privateKey);
        const witness = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.make_vkey_witness(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.hash_transaction(this.txComplete.body()), priv);
        this.witnessSetBuilder.add_vkey(witness);
        const witnesses = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionWitnessSetBuilder["new"]();
        witnesses.add_vkey(witness);
        return (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_2__.toHex)(witnesses.build().to_bytes());
    }
    /** Sign the transaction with the given witnesses. */
    assemble(witnesses) {
        witnesses.forEach((witness) => {
            const witnessParsed = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionWitnessSet.from_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_2__.fromHex)(witness));
            this.witnessSetBuilder.add_existing(witnessParsed);
        });
        return this;
    }
    async complete() {
        for (const task of this.tasks) {
            await task();
        }
        this.witnessSetBuilder.add_existing(this.txComplete.witness_set());
        const signedTx = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Transaction["new"](this.txComplete.body(), this.witnessSetBuilder.build(), this.txComplete.auxiliary_data());
        return new _tx_signed_js__WEBPACK_IMPORTED_MODULE_1__.TxSigned(this.lucid, signedTx);
    }
    /** Return the transaction in Hex encoded Cbor. */
    toString() {
        return (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_2__.toHex)(this.txComplete.to_bytes());
    }
    /** Return the transaction hash. */
    toHash() {
        return _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.hash_transaction(this.txComplete.body()).to_hex();
    }
}

__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/lucid/tx_signed.js":
/*!***************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/lucid/tx_signed.js ***!
  \***************************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "TxSigned": () => (/* binding */ TxSigned)
/* harmony export */ });
/* harmony import */ var _core_mod_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../core/mod.js */ "./node_modules/lucid-cardano/esm/src/core/mod.js");
/* harmony import */ var _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../utils/mod.js */ "./node_modules/lucid-cardano/esm/src/utils/mod.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_core_mod_js__WEBPACK_IMPORTED_MODULE_0__, _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__]);
([_core_mod_js__WEBPACK_IMPORTED_MODULE_0__, _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__] = __webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__);


class TxSigned {
    constructor(lucid, tx) {
        Object.defineProperty(this, "txSigned", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "lucid", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        this.lucid = lucid;
        this.txSigned = tx;
    }
    async submit() {
        return await (this.lucid.wallet || this.lucid.provider).submitTx((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.toHex)(this.txSigned.to_bytes()));
    }
    /** Returns the transaction in Hex encoded Cbor. */
    toString() {
        return (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.toHex)(this.txSigned.to_bytes());
    }
    /** Return the transaction hash. */
    toHash() {
        return _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.hash_transaction(this.txSigned.body()).to_hex();
    }
}

__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/misc/bip39.js":
/*!**********************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/misc/bip39.js ***!
  \**********************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "generateMnemonic": () => (/* binding */ generateMnemonic),
/* harmony export */   "mnemonicToEntropy": () => (/* binding */ mnemonicToEntropy)
/* harmony export */ });
/* harmony import */ var _deps_deno_land_std_0_153_0_hash_sha256_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../../deps/deno.land/std@0.153.0/hash/sha256.js */ "./node_modules/lucid-cardano/esm/deps/deno.land/std@0.153.0/hash/sha256.js");
/* harmony import */ var _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../utils/mod.js */ "./node_modules/lucid-cardano/esm/src/utils/mod.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__]);
_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__ = (__webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__)[0];
// This is a partial reimplementation of BIP39 in Deno: https://github.com/bitcoinjs/bip39
// We only use the default Wordlist (english)


const INVALID_MNEMONIC = "Invalid mnemonic";
const INVALID_ENTROPY = "Invalid entropy";
const INVALID_CHECKSUM = "Invalid mnemonic checksum";
const WORDLIST_REQUIRED = "A wordlist is required but a default could not be found.\n" +
    "Please pass a 2048 word array explicitly.";
function mnemonicToEntropy(mnemonic, wordlist) {
    wordlist = wordlist || DEFAULT_WORDLIST;
    if (!wordlist) {
        throw new Error(WORDLIST_REQUIRED);
    }
    const words = normalize(mnemonic).split(" ");
    if (words.length % 3 !== 0) {
        throw new Error(INVALID_MNEMONIC);
    }
    // convert word indices to 11 bit binary strings
    const bits = words
        .map((word) => {
        const index = wordlist.indexOf(word);
        if (index === -1) {
            throw new Error(INVALID_MNEMONIC);
        }
        return lpad(index.toString(2), "0", 11);
    })
        .join("");
    // split the binary string into ENT/CS
    const dividerIndex = Math.floor(bits.length / 33) * 32;
    const entropyBits = bits.slice(0, dividerIndex);
    const checksumBits = bits.slice(dividerIndex);
    // calculate the checksum and compare
    const entropyBytes = entropyBits.match(/(.{1,8})/g).map(binaryToByte);
    if (entropyBytes.length < 16) {
        throw new Error(INVALID_ENTROPY);
    }
    if (entropyBytes.length > 32) {
        throw new Error(INVALID_ENTROPY);
    }
    if (entropyBytes.length % 4 !== 0) {
        throw new Error(INVALID_ENTROPY);
    }
    const entropy = new Uint8Array(entropyBytes);
    const newChecksum = deriveChecksumBits(entropy);
    if (newChecksum !== checksumBits) {
        throw new Error(INVALID_CHECKSUM);
    }
    return (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.toHex)(entropy);
}
function randomBytes(size) {
    // reimplementation of: https://github.com/crypto-browserify/randombytes/blob/master/browser.js
    const MAX_UINT32 = 4294967295;
    const MAX_BYTES = 65536;
    const bytes = new Uint8Array(size);
    if (size > MAX_UINT32) {
        throw new RangeError("requested too many random bytes");
    }
    if (size > 0) { // getRandomValues fails on IE if size == 0
        if (size > MAX_BYTES) { // this is the max bytes crypto.getRandomValues
            // can do at once see https://developer.mozilla.org/en-US/docs/Web/API/window.crypto.getRandomValues
            for (let generated = 0; generated < size; generated += MAX_BYTES) {
                // buffer.slice automatically checks if the end is past the end of
                // the buffer so we don't have to here
                crypto.getRandomValues(bytes.slice(generated, generated + MAX_BYTES));
            }
        }
        else {
            crypto.getRandomValues(bytes);
        }
    }
    return bytes;
}
function generateMnemonic(strength, rng, wordlist) {
    strength = strength || 128;
    if (strength % 32 !== 0) {
        throw new TypeError(INVALID_ENTROPY);
    }
    rng = rng || randomBytes;
    return entropyToMnemonic(rng(strength / 8), wordlist);
}
function entropyToMnemonic(entropy, wordlist) {
    wordlist = wordlist || DEFAULT_WORDLIST;
    if (!wordlist) {
        throw new Error(WORDLIST_REQUIRED);
    }
    // 128 <= ENT <= 256
    if (entropy.length < 16) {
        throw new TypeError(INVALID_ENTROPY);
    }
    if (entropy.length > 32) {
        throw new TypeError(INVALID_ENTROPY);
    }
    if (entropy.length % 4 !== 0) {
        throw new TypeError(INVALID_ENTROPY);
    }
    const entropyBits = bytesToBinary(Array.from(entropy));
    const checksumBits = deriveChecksumBits(entropy);
    const bits = entropyBits + checksumBits;
    const chunks = bits.match(/(.{1,11})/g);
    const words = chunks.map((binary) => {
        const index = binaryToByte(binary);
        return wordlist[index];
    });
    return wordlist[0] === "\u3042\u3044\u3053\u304f\u3057\u3093" // Japanese wordlist
        ? words.join("\u3000")
        : words.join(" ");
}
function deriveChecksumBits(entropyBuffer) {
    const ENT = entropyBuffer.length * 8;
    const CS = ENT / 32;
    const hash = new _deps_deno_land_std_0_153_0_hash_sha256_js__WEBPACK_IMPORTED_MODULE_0__.Sha256()
        .update(entropyBuffer)
        .digest();
    return bytesToBinary(Array.from(hash)).slice(0, CS);
}
function lpad(str, padString, length) {
    while (str.length < length) {
        str = padString + str;
    }
    return str;
}
function bytesToBinary(bytes) {
    return bytes.map((x) => lpad(x.toString(2), "0", 8)).join("");
}
function normalize(str) {
    return (str || "").normalize("NFKD");
}
function binaryToByte(bin) {
    return parseInt(bin, 2);
}
const DEFAULT_WORDLIST = [
    "abandon",
    "ability",
    "able",
    "about",
    "above",
    "absent",
    "absorb",
    "abstract",
    "absurd",
    "abuse",
    "access",
    "accident",
    "account",
    "accuse",
    "achieve",
    "acid",
    "acoustic",
    "acquire",
    "across",
    "act",
    "action",
    "actor",
    "actress",
    "actual",
    "adapt",
    "add",
    "addict",
    "address",
    "adjust",
    "admit",
    "adult",
    "advance",
    "advice",
    "aerobic",
    "affair",
    "afford",
    "afraid",
    "again",
    "age",
    "agent",
    "agree",
    "ahead",
    "aim",
    "air",
    "airport",
    "aisle",
    "alarm",
    "album",
    "alcohol",
    "alert",
    "alien",
    "all",
    "alley",
    "allow",
    "almost",
    "alone",
    "alpha",
    "already",
    "also",
    "alter",
    "always",
    "amateur",
    "amazing",
    "among",
    "amount",
    "amused",
    "analyst",
    "anchor",
    "ancient",
    "anger",
    "angle",
    "angry",
    "animal",
    "ankle",
    "announce",
    "annual",
    "another",
    "answer",
    "antenna",
    "antique",
    "anxiety",
    "any",
    "apart",
    "apology",
    "appear",
    "apple",
    "approve",
    "april",
    "arch",
    "arctic",
    "area",
    "arena",
    "argue",
    "arm",
    "armed",
    "armor",
    "army",
    "around",
    "arrange",
    "arrest",
    "arrive",
    "arrow",
    "art",
    "artefact",
    "artist",
    "artwork",
    "ask",
    "aspect",
    "assault",
    "asset",
    "assist",
    "assume",
    "asthma",
    "athlete",
    "atom",
    "attack",
    "attend",
    "attitude",
    "attract",
    "auction",
    "audit",
    "august",
    "aunt",
    "author",
    "auto",
    "autumn",
    "average",
    "avocado",
    "avoid",
    "awake",
    "aware",
    "away",
    "awesome",
    "awful",
    "awkward",
    "axis",
    "baby",
    "bachelor",
    "bacon",
    "badge",
    "bag",
    "balance",
    "balcony",
    "ball",
    "bamboo",
    "banana",
    "banner",
    "bar",
    "barely",
    "bargain",
    "barrel",
    "base",
    "basic",
    "basket",
    "battle",
    "beach",
    "bean",
    "beauty",
    "because",
    "become",
    "beef",
    "before",
    "begin",
    "behave",
    "behind",
    "believe",
    "below",
    "belt",
    "bench",
    "benefit",
    "best",
    "betray",
    "better",
    "between",
    "beyond",
    "bicycle",
    "bid",
    "bike",
    "bind",
    "biology",
    "bird",
    "birth",
    "bitter",
    "black",
    "blade",
    "blame",
    "blanket",
    "blast",
    "bleak",
    "bless",
    "blind",
    "blood",
    "blossom",
    "blouse",
    "blue",
    "blur",
    "blush",
    "board",
    "boat",
    "body",
    "boil",
    "bomb",
    "bone",
    "bonus",
    "book",
    "boost",
    "border",
    "boring",
    "borrow",
    "boss",
    "bottom",
    "bounce",
    "box",
    "boy",
    "bracket",
    "brain",
    "brand",
    "brass",
    "brave",
    "bread",
    "breeze",
    "brick",
    "bridge",
    "brief",
    "bright",
    "bring",
    "brisk",
    "broccoli",
    "broken",
    "bronze",
    "broom",
    "brother",
    "brown",
    "brush",
    "bubble",
    "buddy",
    "budget",
    "buffalo",
    "build",
    "bulb",
    "bulk",
    "bullet",
    "bundle",
    "bunker",
    "burden",
    "burger",
    "burst",
    "bus",
    "business",
    "busy",
    "butter",
    "buyer",
    "buzz",
    "cabbage",
    "cabin",
    "cable",
    "cactus",
    "cage",
    "cake",
    "call",
    "calm",
    "camera",
    "camp",
    "can",
    "canal",
    "cancel",
    "candy",
    "cannon",
    "canoe",
    "canvas",
    "canyon",
    "capable",
    "capital",
    "captain",
    "car",
    "carbon",
    "card",
    "cargo",
    "carpet",
    "carry",
    "cart",
    "case",
    "cash",
    "casino",
    "castle",
    "casual",
    "cat",
    "catalog",
    "catch",
    "category",
    "cattle",
    "caught",
    "cause",
    "caution",
    "cave",
    "ceiling",
    "celery",
    "cement",
    "census",
    "century",
    "cereal",
    "certain",
    "chair",
    "chalk",
    "champion",
    "change",
    "chaos",
    "chapter",
    "charge",
    "chase",
    "chat",
    "cheap",
    "check",
    "cheese",
    "chef",
    "cherry",
    "chest",
    "chicken",
    "chief",
    "child",
    "chimney",
    "choice",
    "choose",
    "chronic",
    "chuckle",
    "chunk",
    "churn",
    "cigar",
    "cinnamon",
    "circle",
    "citizen",
    "city",
    "civil",
    "claim",
    "clap",
    "clarify",
    "claw",
    "clay",
    "clean",
    "clerk",
    "clever",
    "click",
    "client",
    "cliff",
    "climb",
    "clinic",
    "clip",
    "clock",
    "clog",
    "close",
    "cloth",
    "cloud",
    "clown",
    "club",
    "clump",
    "cluster",
    "clutch",
    "coach",
    "coast",
    "coconut",
    "code",
    "coffee",
    "coil",
    "coin",
    "collect",
    "color",
    "column",
    "combine",
    "come",
    "comfort",
    "comic",
    "common",
    "company",
    "concert",
    "conduct",
    "confirm",
    "congress",
    "connect",
    "consider",
    "control",
    "convince",
    "cook",
    "cool",
    "copper",
    "copy",
    "coral",
    "core",
    "corn",
    "correct",
    "cost",
    "cotton",
    "couch",
    "country",
    "couple",
    "course",
    "cousin",
    "cover",
    "coyote",
    "crack",
    "cradle",
    "craft",
    "cram",
    "crane",
    "crash",
    "crater",
    "crawl",
    "crazy",
    "cream",
    "credit",
    "creek",
    "crew",
    "cricket",
    "crime",
    "crisp",
    "critic",
    "crop",
    "cross",
    "crouch",
    "crowd",
    "crucial",
    "cruel",
    "cruise",
    "crumble",
    "crunch",
    "crush",
    "cry",
    "crystal",
    "cube",
    "culture",
    "cup",
    "cupboard",
    "curious",
    "current",
    "curtain",
    "curve",
    "cushion",
    "custom",
    "cute",
    "cycle",
    "dad",
    "damage",
    "damp",
    "dance",
    "danger",
    "daring",
    "dash",
    "daughter",
    "dawn",
    "day",
    "deal",
    "debate",
    "debris",
    "decade",
    "december",
    "decide",
    "decline",
    "decorate",
    "decrease",
    "deer",
    "defense",
    "define",
    "defy",
    "degree",
    "delay",
    "deliver",
    "demand",
    "demise",
    "denial",
    "dentist",
    "deny",
    "depart",
    "depend",
    "deposit",
    "depth",
    "deputy",
    "derive",
    "describe",
    "desert",
    "design",
    "desk",
    "despair",
    "destroy",
    "detail",
    "detect",
    "develop",
    "device",
    "devote",
    "diagram",
    "dial",
    "diamond",
    "diary",
    "dice",
    "diesel",
    "diet",
    "differ",
    "digital",
    "dignity",
    "dilemma",
    "dinner",
    "dinosaur",
    "direct",
    "dirt",
    "disagree",
    "discover",
    "disease",
    "dish",
    "dismiss",
    "disorder",
    "display",
    "distance",
    "divert",
    "divide",
    "divorce",
    "dizzy",
    "doctor",
    "document",
    "dog",
    "doll",
    "dolphin",
    "domain",
    "donate",
    "donkey",
    "donor",
    "door",
    "dose",
    "double",
    "dove",
    "draft",
    "dragon",
    "drama",
    "drastic",
    "draw",
    "dream",
    "dress",
    "drift",
    "drill",
    "drink",
    "drip",
    "drive",
    "drop",
    "drum",
    "dry",
    "duck",
    "dumb",
    "dune",
    "during",
    "dust",
    "dutch",
    "duty",
    "dwarf",
    "dynamic",
    "eager",
    "eagle",
    "early",
    "earn",
    "earth",
    "easily",
    "east",
    "easy",
    "echo",
    "ecology",
    "economy",
    "edge",
    "edit",
    "educate",
    "effort",
    "egg",
    "eight",
    "either",
    "elbow",
    "elder",
    "electric",
    "elegant",
    "element",
    "elephant",
    "elevator",
    "elite",
    "else",
    "embark",
    "embody",
    "embrace",
    "emerge",
    "emotion",
    "employ",
    "empower",
    "empty",
    "enable",
    "enact",
    "end",
    "endless",
    "endorse",
    "enemy",
    "energy",
    "enforce",
    "engage",
    "engine",
    "enhance",
    "enjoy",
    "enlist",
    "enough",
    "enrich",
    "enroll",
    "ensure",
    "enter",
    "entire",
    "entry",
    "envelope",
    "episode",
    "equal",
    "equip",
    "era",
    "erase",
    "erode",
    "erosion",
    "error",
    "erupt",
    "escape",
    "essay",
    "essence",
    "estate",
    "eternal",
    "ethics",
    "evidence",
    "evil",
    "evoke",
    "evolve",
    "exact",
    "example",
    "excess",
    "exchange",
    "excite",
    "exclude",
    "excuse",
    "execute",
    "exercise",
    "exhaust",
    "exhibit",
    "exile",
    "exist",
    "exit",
    "exotic",
    "expand",
    "expect",
    "expire",
    "explain",
    "expose",
    "express",
    "extend",
    "extra",
    "eye",
    "eyebrow",
    "fabric",
    "face",
    "faculty",
    "fade",
    "faint",
    "faith",
    "fall",
    "false",
    "fame",
    "family",
    "famous",
    "fan",
    "fancy",
    "fantasy",
    "farm",
    "fashion",
    "fat",
    "fatal",
    "father",
    "fatigue",
    "fault",
    "favorite",
    "feature",
    "february",
    "federal",
    "fee",
    "feed",
    "feel",
    "female",
    "fence",
    "festival",
    "fetch",
    "fever",
    "few",
    "fiber",
    "fiction",
    "field",
    "figure",
    "file",
    "film",
    "filter",
    "final",
    "find",
    "fine",
    "finger",
    "finish",
    "fire",
    "firm",
    "first",
    "fiscal",
    "fish",
    "fit",
    "fitness",
    "fix",
    "flag",
    "flame",
    "flash",
    "flat",
    "flavor",
    "flee",
    "flight",
    "flip",
    "float",
    "flock",
    "floor",
    "flower",
    "fluid",
    "flush",
    "fly",
    "foam",
    "focus",
    "fog",
    "foil",
    "fold",
    "follow",
    "food",
    "foot",
    "force",
    "forest",
    "forget",
    "fork",
    "fortune",
    "forum",
    "forward",
    "fossil",
    "foster",
    "found",
    "fox",
    "fragile",
    "frame",
    "frequent",
    "fresh",
    "friend",
    "fringe",
    "frog",
    "front",
    "frost",
    "frown",
    "frozen",
    "fruit",
    "fuel",
    "fun",
    "funny",
    "furnace",
    "fury",
    "future",
    "gadget",
    "gain",
    "galaxy",
    "gallery",
    "game",
    "gap",
    "garage",
    "garbage",
    "garden",
    "garlic",
    "garment",
    "gas",
    "gasp",
    "gate",
    "gather",
    "gauge",
    "gaze",
    "general",
    "genius",
    "genre",
    "gentle",
    "genuine",
    "gesture",
    "ghost",
    "giant",
    "gift",
    "giggle",
    "ginger",
    "giraffe",
    "girl",
    "give",
    "glad",
    "glance",
    "glare",
    "glass",
    "glide",
    "glimpse",
    "globe",
    "gloom",
    "glory",
    "glove",
    "glow",
    "glue",
    "goat",
    "goddess",
    "gold",
    "good",
    "goose",
    "gorilla",
    "gospel",
    "gossip",
    "govern",
    "gown",
    "grab",
    "grace",
    "grain",
    "grant",
    "grape",
    "grass",
    "gravity",
    "great",
    "green",
    "grid",
    "grief",
    "grit",
    "grocery",
    "group",
    "grow",
    "grunt",
    "guard",
    "guess",
    "guide",
    "guilt",
    "guitar",
    "gun",
    "gym",
    "habit",
    "hair",
    "half",
    "hammer",
    "hamster",
    "hand",
    "happy",
    "harbor",
    "hard",
    "harsh",
    "harvest",
    "hat",
    "have",
    "hawk",
    "hazard",
    "head",
    "health",
    "heart",
    "heavy",
    "hedgehog",
    "height",
    "hello",
    "helmet",
    "help",
    "hen",
    "hero",
    "hidden",
    "high",
    "hill",
    "hint",
    "hip",
    "hire",
    "history",
    "hobby",
    "hockey",
    "hold",
    "hole",
    "holiday",
    "hollow",
    "home",
    "honey",
    "hood",
    "hope",
    "horn",
    "horror",
    "horse",
    "hospital",
    "host",
    "hotel",
    "hour",
    "hover",
    "hub",
    "huge",
    "human",
    "humble",
    "humor",
    "hundred",
    "hungry",
    "hunt",
    "hurdle",
    "hurry",
    "hurt",
    "husband",
    "hybrid",
    "ice",
    "icon",
    "idea",
    "identify",
    "idle",
    "ignore",
    "ill",
    "illegal",
    "illness",
    "image",
    "imitate",
    "immense",
    "immune",
    "impact",
    "impose",
    "improve",
    "impulse",
    "inch",
    "include",
    "income",
    "increase",
    "index",
    "indicate",
    "indoor",
    "industry",
    "infant",
    "inflict",
    "inform",
    "inhale",
    "inherit",
    "initial",
    "inject",
    "injury",
    "inmate",
    "inner",
    "innocent",
    "input",
    "inquiry",
    "insane",
    "insect",
    "inside",
    "inspire",
    "install",
    "intact",
    "interest",
    "into",
    "invest",
    "invite",
    "involve",
    "iron",
    "island",
    "isolate",
    "issue",
    "item",
    "ivory",
    "jacket",
    "jaguar",
    "jar",
    "jazz",
    "jealous",
    "jeans",
    "jelly",
    "jewel",
    "job",
    "join",
    "joke",
    "journey",
    "joy",
    "judge",
    "juice",
    "jump",
    "jungle",
    "junior",
    "junk",
    "just",
    "kangaroo",
    "keen",
    "keep",
    "ketchup",
    "key",
    "kick",
    "kid",
    "kidney",
    "kind",
    "kingdom",
    "kiss",
    "kit",
    "kitchen",
    "kite",
    "kitten",
    "kiwi",
    "knee",
    "knife",
    "knock",
    "know",
    "lab",
    "label",
    "labor",
    "ladder",
    "lady",
    "lake",
    "lamp",
    "language",
    "laptop",
    "large",
    "later",
    "latin",
    "laugh",
    "laundry",
    "lava",
    "law",
    "lawn",
    "lawsuit",
    "layer",
    "lazy",
    "leader",
    "leaf",
    "learn",
    "leave",
    "lecture",
    "left",
    "leg",
    "legal",
    "legend",
    "leisure",
    "lemon",
    "lend",
    "length",
    "lens",
    "leopard",
    "lesson",
    "letter",
    "level",
    "liar",
    "liberty",
    "library",
    "license",
    "life",
    "lift",
    "light",
    "like",
    "limb",
    "limit",
    "link",
    "lion",
    "liquid",
    "list",
    "little",
    "live",
    "lizard",
    "load",
    "loan",
    "lobster",
    "local",
    "lock",
    "logic",
    "lonely",
    "long",
    "loop",
    "lottery",
    "loud",
    "lounge",
    "love",
    "loyal",
    "lucky",
    "luggage",
    "lumber",
    "lunar",
    "lunch",
    "luxury",
    "lyrics",
    "machine",
    "mad",
    "magic",
    "magnet",
    "maid",
    "mail",
    "main",
    "major",
    "make",
    "mammal",
    "man",
    "manage",
    "mandate",
    "mango",
    "mansion",
    "manual",
    "maple",
    "marble",
    "march",
    "margin",
    "marine",
    "market",
    "marriage",
    "mask",
    "mass",
    "master",
    "match",
    "material",
    "math",
    "matrix",
    "matter",
    "maximum",
    "maze",
    "meadow",
    "mean",
    "measure",
    "meat",
    "mechanic",
    "medal",
    "media",
    "melody",
    "melt",
    "member",
    "memory",
    "mention",
    "menu",
    "mercy",
    "merge",
    "merit",
    "merry",
    "mesh",
    "message",
    "metal",
    "method",
    "middle",
    "midnight",
    "milk",
    "million",
    "mimic",
    "mind",
    "minimum",
    "minor",
    "minute",
    "miracle",
    "mirror",
    "misery",
    "miss",
    "mistake",
    "mix",
    "mixed",
    "mixture",
    "mobile",
    "model",
    "modify",
    "mom",
    "moment",
    "monitor",
    "monkey",
    "monster",
    "month",
    "moon",
    "moral",
    "more",
    "morning",
    "mosquito",
    "mother",
    "motion",
    "motor",
    "mountain",
    "mouse",
    "move",
    "movie",
    "much",
    "muffin",
    "mule",
    "multiply",
    "muscle",
    "museum",
    "mushroom",
    "music",
    "must",
    "mutual",
    "myself",
    "mystery",
    "myth",
    "naive",
    "name",
    "napkin",
    "narrow",
    "nasty",
    "nation",
    "nature",
    "near",
    "neck",
    "need",
    "negative",
    "neglect",
    "neither",
    "nephew",
    "nerve",
    "nest",
    "net",
    "network",
    "neutral",
    "never",
    "news",
    "next",
    "nice",
    "night",
    "noble",
    "noise",
    "nominee",
    "noodle",
    "normal",
    "north",
    "nose",
    "notable",
    "note",
    "nothing",
    "notice",
    "novel",
    "now",
    "nuclear",
    "number",
    "nurse",
    "nut",
    "oak",
    "obey",
    "object",
    "oblige",
    "obscure",
    "observe",
    "obtain",
    "obvious",
    "occur",
    "ocean",
    "october",
    "odor",
    "off",
    "offer",
    "office",
    "often",
    "oil",
    "okay",
    "old",
    "olive",
    "olympic",
    "omit",
    "once",
    "one",
    "onion",
    "online",
    "only",
    "open",
    "opera",
    "opinion",
    "oppose",
    "option",
    "orange",
    "orbit",
    "orchard",
    "order",
    "ordinary",
    "organ",
    "orient",
    "original",
    "orphan",
    "ostrich",
    "other",
    "outdoor",
    "outer",
    "output",
    "outside",
    "oval",
    "oven",
    "over",
    "own",
    "owner",
    "oxygen",
    "oyster",
    "ozone",
    "pact",
    "paddle",
    "page",
    "pair",
    "palace",
    "palm",
    "panda",
    "panel",
    "panic",
    "panther",
    "paper",
    "parade",
    "parent",
    "park",
    "parrot",
    "party",
    "pass",
    "patch",
    "path",
    "patient",
    "patrol",
    "pattern",
    "pause",
    "pave",
    "payment",
    "peace",
    "peanut",
    "pear",
    "peasant",
    "pelican",
    "pen",
    "penalty",
    "pencil",
    "people",
    "pepper",
    "perfect",
    "permit",
    "person",
    "pet",
    "phone",
    "photo",
    "phrase",
    "physical",
    "piano",
    "picnic",
    "picture",
    "piece",
    "pig",
    "pigeon",
    "pill",
    "pilot",
    "pink",
    "pioneer",
    "pipe",
    "pistol",
    "pitch",
    "pizza",
    "place",
    "planet",
    "plastic",
    "plate",
    "play",
    "please",
    "pledge",
    "pluck",
    "plug",
    "plunge",
    "poem",
    "poet",
    "point",
    "polar",
    "pole",
    "police",
    "pond",
    "pony",
    "pool",
    "popular",
    "portion",
    "position",
    "possible",
    "post",
    "potato",
    "pottery",
    "poverty",
    "powder",
    "power",
    "practice",
    "praise",
    "predict",
    "prefer",
    "prepare",
    "present",
    "pretty",
    "prevent",
    "price",
    "pride",
    "primary",
    "print",
    "priority",
    "prison",
    "private",
    "prize",
    "problem",
    "process",
    "produce",
    "profit",
    "program",
    "project",
    "promote",
    "proof",
    "property",
    "prosper",
    "protect",
    "proud",
    "provide",
    "public",
    "pudding",
    "pull",
    "pulp",
    "pulse",
    "pumpkin",
    "punch",
    "pupil",
    "puppy",
    "purchase",
    "purity",
    "purpose",
    "purse",
    "push",
    "put",
    "puzzle",
    "pyramid",
    "quality",
    "quantum",
    "quarter",
    "question",
    "quick",
    "quit",
    "quiz",
    "quote",
    "rabbit",
    "raccoon",
    "race",
    "rack",
    "radar",
    "radio",
    "rail",
    "rain",
    "raise",
    "rally",
    "ramp",
    "ranch",
    "random",
    "range",
    "rapid",
    "rare",
    "rate",
    "rather",
    "raven",
    "raw",
    "razor",
    "ready",
    "real",
    "reason",
    "rebel",
    "rebuild",
    "recall",
    "receive",
    "recipe",
    "record",
    "recycle",
    "reduce",
    "reflect",
    "reform",
    "refuse",
    "region",
    "regret",
    "regular",
    "reject",
    "relax",
    "release",
    "relief",
    "rely",
    "remain",
    "remember",
    "remind",
    "remove",
    "render",
    "renew",
    "rent",
    "reopen",
    "repair",
    "repeat",
    "replace",
    "report",
    "require",
    "rescue",
    "resemble",
    "resist",
    "resource",
    "response",
    "result",
    "retire",
    "retreat",
    "return",
    "reunion",
    "reveal",
    "review",
    "reward",
    "rhythm",
    "rib",
    "ribbon",
    "rice",
    "rich",
    "ride",
    "ridge",
    "rifle",
    "right",
    "rigid",
    "ring",
    "riot",
    "ripple",
    "risk",
    "ritual",
    "rival",
    "river",
    "road",
    "roast",
    "robot",
    "robust",
    "rocket",
    "romance",
    "roof",
    "rookie",
    "room",
    "rose",
    "rotate",
    "rough",
    "round",
    "route",
    "royal",
    "rubber",
    "rude",
    "rug",
    "rule",
    "run",
    "runway",
    "rural",
    "sad",
    "saddle",
    "sadness",
    "safe",
    "sail",
    "salad",
    "salmon",
    "salon",
    "salt",
    "salute",
    "same",
    "sample",
    "sand",
    "satisfy",
    "satoshi",
    "sauce",
    "sausage",
    "save",
    "say",
    "scale",
    "scan",
    "scare",
    "scatter",
    "scene",
    "scheme",
    "school",
    "science",
    "scissors",
    "scorpion",
    "scout",
    "scrap",
    "screen",
    "script",
    "scrub",
    "sea",
    "search",
    "season",
    "seat",
    "second",
    "secret",
    "section",
    "security",
    "seed",
    "seek",
    "segment",
    "select",
    "sell",
    "seminar",
    "senior",
    "sense",
    "sentence",
    "series",
    "service",
    "session",
    "settle",
    "setup",
    "seven",
    "shadow",
    "shaft",
    "shallow",
    "share",
    "shed",
    "shell",
    "sheriff",
    "shield",
    "shift",
    "shine",
    "ship",
    "shiver",
    "shock",
    "shoe",
    "shoot",
    "shop",
    "short",
    "shoulder",
    "shove",
    "shrimp",
    "shrug",
    "shuffle",
    "shy",
    "sibling",
    "sick",
    "side",
    "siege",
    "sight",
    "sign",
    "silent",
    "silk",
    "silly",
    "silver",
    "similar",
    "simple",
    "since",
    "sing",
    "siren",
    "sister",
    "situate",
    "six",
    "size",
    "skate",
    "sketch",
    "ski",
    "skill",
    "skin",
    "skirt",
    "skull",
    "slab",
    "slam",
    "sleep",
    "slender",
    "slice",
    "slide",
    "slight",
    "slim",
    "slogan",
    "slot",
    "slow",
    "slush",
    "small",
    "smart",
    "smile",
    "smoke",
    "smooth",
    "snack",
    "snake",
    "snap",
    "sniff",
    "snow",
    "soap",
    "soccer",
    "social",
    "sock",
    "soda",
    "soft",
    "solar",
    "soldier",
    "solid",
    "solution",
    "solve",
    "someone",
    "song",
    "soon",
    "sorry",
    "sort",
    "soul",
    "sound",
    "soup",
    "source",
    "south",
    "space",
    "spare",
    "spatial",
    "spawn",
    "speak",
    "special",
    "speed",
    "spell",
    "spend",
    "sphere",
    "spice",
    "spider",
    "spike",
    "spin",
    "spirit",
    "split",
    "spoil",
    "sponsor",
    "spoon",
    "sport",
    "spot",
    "spray",
    "spread",
    "spring",
    "spy",
    "square",
    "squeeze",
    "squirrel",
    "stable",
    "stadium",
    "staff",
    "stage",
    "stairs",
    "stamp",
    "stand",
    "start",
    "state",
    "stay",
    "steak",
    "steel",
    "stem",
    "step",
    "stereo",
    "stick",
    "still",
    "sting",
    "stock",
    "stomach",
    "stone",
    "stool",
    "story",
    "stove",
    "strategy",
    "street",
    "strike",
    "strong",
    "struggle",
    "student",
    "stuff",
    "stumble",
    "style",
    "subject",
    "submit",
    "subway",
    "success",
    "such",
    "sudden",
    "suffer",
    "sugar",
    "suggest",
    "suit",
    "summer",
    "sun",
    "sunny",
    "sunset",
    "super",
    "supply",
    "supreme",
    "sure",
    "surface",
    "surge",
    "surprise",
    "surround",
    "survey",
    "suspect",
    "sustain",
    "swallow",
    "swamp",
    "swap",
    "swarm",
    "swear",
    "sweet",
    "swift",
    "swim",
    "swing",
    "switch",
    "sword",
    "symbol",
    "symptom",
    "syrup",
    "system",
    "table",
    "tackle",
    "tag",
    "tail",
    "talent",
    "talk",
    "tank",
    "tape",
    "target",
    "task",
    "taste",
    "tattoo",
    "taxi",
    "teach",
    "team",
    "tell",
    "ten",
    "tenant",
    "tennis",
    "tent",
    "term",
    "test",
    "text",
    "thank",
    "that",
    "theme",
    "then",
    "theory",
    "there",
    "they",
    "thing",
    "this",
    "thought",
    "three",
    "thrive",
    "throw",
    "thumb",
    "thunder",
    "ticket",
    "tide",
    "tiger",
    "tilt",
    "timber",
    "time",
    "tiny",
    "tip",
    "tired",
    "tissue",
    "title",
    "toast",
    "tobacco",
    "today",
    "toddler",
    "toe",
    "together",
    "toilet",
    "token",
    "tomato",
    "tomorrow",
    "tone",
    "tongue",
    "tonight",
    "tool",
    "tooth",
    "top",
    "topic",
    "topple",
    "torch",
    "tornado",
    "tortoise",
    "toss",
    "total",
    "tourist",
    "toward",
    "tower",
    "town",
    "toy",
    "track",
    "trade",
    "traffic",
    "tragic",
    "train",
    "transfer",
    "trap",
    "trash",
    "travel",
    "tray",
    "treat",
    "tree",
    "trend",
    "trial",
    "tribe",
    "trick",
    "trigger",
    "trim",
    "trip",
    "trophy",
    "trouble",
    "truck",
    "true",
    "truly",
    "trumpet",
    "trust",
    "truth",
    "try",
    "tube",
    "tuition",
    "tumble",
    "tuna",
    "tunnel",
    "turkey",
    "turn",
    "turtle",
    "twelve",
    "twenty",
    "twice",
    "twin",
    "twist",
    "two",
    "type",
    "typical",
    "ugly",
    "umbrella",
    "unable",
    "unaware",
    "uncle",
    "uncover",
    "under",
    "undo",
    "unfair",
    "unfold",
    "unhappy",
    "uniform",
    "unique",
    "unit",
    "universe",
    "unknown",
    "unlock",
    "until",
    "unusual",
    "unveil",
    "update",
    "upgrade",
    "uphold",
    "upon",
    "upper",
    "upset",
    "urban",
    "urge",
    "usage",
    "use",
    "used",
    "useful",
    "useless",
    "usual",
    "utility",
    "vacant",
    "vacuum",
    "vague",
    "valid",
    "valley",
    "valve",
    "van",
    "vanish",
    "vapor",
    "various",
    "vast",
    "vault",
    "vehicle",
    "velvet",
    "vendor",
    "venture",
    "venue",
    "verb",
    "verify",
    "version",
    "very",
    "vessel",
    "veteran",
    "viable",
    "vibrant",
    "vicious",
    "victory",
    "video",
    "view",
    "village",
    "vintage",
    "violin",
    "virtual",
    "virus",
    "visa",
    "visit",
    "visual",
    "vital",
    "vivid",
    "vocal",
    "voice",
    "void",
    "volcano",
    "volume",
    "vote",
    "voyage",
    "wage",
    "wagon",
    "wait",
    "walk",
    "wall",
    "walnut",
    "want",
    "warfare",
    "warm",
    "warrior",
    "wash",
    "wasp",
    "waste",
    "water",
    "wave",
    "way",
    "wealth",
    "weapon",
    "wear",
    "weasel",
    "weather",
    "web",
    "wedding",
    "weekend",
    "weird",
    "welcome",
    "west",
    "wet",
    "whale",
    "what",
    "wheat",
    "wheel",
    "when",
    "where",
    "whip",
    "whisper",
    "wide",
    "width",
    "wife",
    "wild",
    "will",
    "win",
    "window",
    "wine",
    "wing",
    "wink",
    "winner",
    "winter",
    "wire",
    "wisdom",
    "wise",
    "wish",
    "witness",
    "wolf",
    "woman",
    "wonder",
    "wood",
    "wool",
    "word",
    "work",
    "world",
    "worry",
    "worth",
    "wrap",
    "wreck",
    "wrestle",
    "wrist",
    "write",
    "wrong",
    "yard",
    "year",
    "yellow",
    "you",
    "young",
    "youth",
    "zebra",
    "zero",
    "zone",
    "zoo",
];

__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/misc/crc8.js":
/*!*********************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/misc/crc8.js ***!
  \*********************************************************/
/***/ ((__unused_webpack___webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "crc8": () => (/* binding */ crc8)
/* harmony export */ });
// This is a partial reimplementation of CRC-8 (node-crc) in Deno: https://github.com/alexgorbatchev/node-crc
let TABLE = [
    0x00,
    0x07,
    0x0e,
    0x09,
    0x1c,
    0x1b,
    0x12,
    0x15,
    0x38,
    0x3f,
    0x36,
    0x31,
    0x24,
    0x23,
    0x2a,
    0x2d,
    0x70,
    0x77,
    0x7e,
    0x79,
    0x6c,
    0x6b,
    0x62,
    0x65,
    0x48,
    0x4f,
    0x46,
    0x41,
    0x54,
    0x53,
    0x5a,
    0x5d,
    0xe0,
    0xe7,
    0xee,
    0xe9,
    0xfc,
    0xfb,
    0xf2,
    0xf5,
    0xd8,
    0xdf,
    0xd6,
    0xd1,
    0xc4,
    0xc3,
    0xca,
    0xcd,
    0x90,
    0x97,
    0x9e,
    0x99,
    0x8c,
    0x8b,
    0x82,
    0x85,
    0xa8,
    0xaf,
    0xa6,
    0xa1,
    0xb4,
    0xb3,
    0xba,
    0xbd,
    0xc7,
    0xc0,
    0xc9,
    0xce,
    0xdb,
    0xdc,
    0xd5,
    0xd2,
    0xff,
    0xf8,
    0xf1,
    0xf6,
    0xe3,
    0xe4,
    0xed,
    0xea,
    0xb7,
    0xb0,
    0xb9,
    0xbe,
    0xab,
    0xac,
    0xa5,
    0xa2,
    0x8f,
    0x88,
    0x81,
    0x86,
    0x93,
    0x94,
    0x9d,
    0x9a,
    0x27,
    0x20,
    0x29,
    0x2e,
    0x3b,
    0x3c,
    0x35,
    0x32,
    0x1f,
    0x18,
    0x11,
    0x16,
    0x03,
    0x04,
    0x0d,
    0x0a,
    0x57,
    0x50,
    0x59,
    0x5e,
    0x4b,
    0x4c,
    0x45,
    0x42,
    0x6f,
    0x68,
    0x61,
    0x66,
    0x73,
    0x74,
    0x7d,
    0x7a,
    0x89,
    0x8e,
    0x87,
    0x80,
    0x95,
    0x92,
    0x9b,
    0x9c,
    0xb1,
    0xb6,
    0xbf,
    0xb8,
    0xad,
    0xaa,
    0xa3,
    0xa4,
    0xf9,
    0xfe,
    0xf7,
    0xf0,
    0xe5,
    0xe2,
    0xeb,
    0xec,
    0xc1,
    0xc6,
    0xcf,
    0xc8,
    0xdd,
    0xda,
    0xd3,
    0xd4,
    0x69,
    0x6e,
    0x67,
    0x60,
    0x75,
    0x72,
    0x7b,
    0x7c,
    0x51,
    0x56,
    0x5f,
    0x58,
    0x4d,
    0x4a,
    0x43,
    0x44,
    0x19,
    0x1e,
    0x17,
    0x10,
    0x05,
    0x02,
    0x0b,
    0x0c,
    0x21,
    0x26,
    0x2f,
    0x28,
    0x3d,
    0x3a,
    0x33,
    0x34,
    0x4e,
    0x49,
    0x40,
    0x47,
    0x52,
    0x55,
    0x5c,
    0x5b,
    0x76,
    0x71,
    0x78,
    0x7f,
    0x6a,
    0x6d,
    0x64,
    0x63,
    0x3e,
    0x39,
    0x30,
    0x37,
    0x22,
    0x25,
    0x2c,
    0x2b,
    0x06,
    0x01,
    0x08,
    0x0f,
    0x1a,
    0x1d,
    0x14,
    0x13,
    0xae,
    0xa9,
    0xa0,
    0xa7,
    0xb2,
    0xb5,
    0xbc,
    0xbb,
    0x96,
    0x91,
    0x98,
    0x9f,
    0x8a,
    0x8d,
    0x84,
    0x83,
    0xde,
    0xd9,
    0xd0,
    0xd7,
    0xc2,
    0xc5,
    0xcc,
    0xcb,
    0xe6,
    0xe1,
    0xe8,
    0xef,
    0xfa,
    0xfd,
    0xf4,
    0xf3,
];
if (typeof Int32Array !== "undefined") {
    TABLE = new Int32Array(TABLE);
}
function crc8(current, previous = 0) {
    let crc = ~~previous;
    for (let index = 0; index < current.length; index++) {
        crc = TABLE[(crc ^ current[index]) & 0xff] & 0xff;
    }
    return crc;
}


/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/misc/sign_data.js":
/*!**************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/misc/sign_data.js ***!
  \**************************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "signData": () => (/* binding */ signData),
/* harmony export */   "verifyData": () => (/* binding */ verifyData)
/* harmony export */ });
/* harmony import */ var _mod_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../mod.js */ "./node_modules/lucid-cardano/esm/src/mod.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_mod_js__WEBPACK_IMPORTED_MODULE_0__]);
_mod_js__WEBPACK_IMPORTED_MODULE_0__ = (__webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__)[0];

function signData(addressHex, payload, privateKey) {
    const protectedHeaders = _mod_js__WEBPACK_IMPORTED_MODULE_0__.M.HeaderMap["new"]();
    protectedHeaders.set_algorithm_id(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.Label.from_algorithm_id(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.AlgorithmId.EdDSA));
    protectedHeaders.set_header(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.Label.new_text("address"), _mod_js__WEBPACK_IMPORTED_MODULE_0__.M.CBORValue.new_bytes((0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.fromHex)(addressHex)));
    const protectedSerialized = _mod_js__WEBPACK_IMPORTED_MODULE_0__.M.ProtectedHeaderMap["new"](protectedHeaders);
    const unprotectedHeaders = _mod_js__WEBPACK_IMPORTED_MODULE_0__.M.HeaderMap["new"]();
    const headers = _mod_js__WEBPACK_IMPORTED_MODULE_0__.M.Headers["new"](protectedSerialized, unprotectedHeaders);
    const builder = _mod_js__WEBPACK_IMPORTED_MODULE_0__.M.COSESign1Builder["new"](headers, (0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.fromHex)(payload), false);
    const toSign = builder.make_data_to_sign().to_bytes();
    const priv = _mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PrivateKey.from_bech32(privateKey);
    const signedSigStruc = priv.sign(toSign).to_bytes();
    const coseSign1 = builder.build(signedSigStruc);
    const key = _mod_js__WEBPACK_IMPORTED_MODULE_0__.M.COSEKey["new"](_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.Label.from_key_type(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.KeyType.OKP));
    key.set_algorithm_id(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.Label.from_algorithm_id(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.AlgorithmId.EdDSA));
    key.set_header(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.Label.new_int(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.Int.new_negative(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.BigNum.from_str("1"))), _mod_js__WEBPACK_IMPORTED_MODULE_0__.M.CBORValue.new_int(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.Int.new_i32(6))); // crv (-1) set to Ed25519 (6)
    key.set_header(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.Label.new_int(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.Int.new_negative(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.BigNum.from_str("2"))), _mod_js__WEBPACK_IMPORTED_MODULE_0__.M.CBORValue.new_bytes(priv.to_public().as_bytes())); // x (-2) set to public key
    return {
        signature: (0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.toHex)(coseSign1.to_bytes()),
        key: (0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.toHex)(key.to_bytes()),
    };
}
function verifyData(addressHex, keyHash, payload, signedMessage) {
    const cose1 = _mod_js__WEBPACK_IMPORTED_MODULE_0__.M.COSESign1.from_bytes((0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.fromHex)(signedMessage.signature));
    const key = _mod_js__WEBPACK_IMPORTED_MODULE_0__.M.COSEKey.from_bytes((0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.fromHex)(signedMessage.key));
    const protectedHeaders = cose1.headers().protected()
        .deserialized_headers();
    const cose1Address = (() => {
        try {
            return (0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.toHex)(protectedHeaders.header(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.Label.new_text("address"))?.as_bytes());
        }
        catch (_e) {
            throw new Error("No address found in signature.");
        }
    })();
    const cose1AlgorithmId = (() => {
        try {
            const int = protectedHeaders.algorithm_id()?.as_int();
            if (int?.is_positive())
                return parseInt(int.as_positive()?.to_str());
            return parseInt(int?.as_negative()?.to_str());
        }
        catch (_e) {
            throw new Error("Failed to retrieve Algorithm Id.");
        }
    })();
    const keyAlgorithmId = (() => {
        try {
            const int = key.algorithm_id()?.as_int();
            if (int?.is_positive())
                return parseInt(int.as_positive()?.to_str());
            return parseInt(int?.as_negative()?.to_str());
        }
        catch (_e) {
            throw new Error("Failed to retrieve Algorithm Id.");
        }
    })();
    const keyCurve = (() => {
        try {
            const int = key.header(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.Label.new_int(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.Int.new_negative(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.BigNum.from_str("1"))))?.as_int();
            if (int?.is_positive())
                return parseInt(int.as_positive()?.to_str());
            return parseInt(int?.as_negative()?.to_str());
        }
        catch (_e) {
            throw new Error("Failed to retrieve Curve.");
        }
    })();
    const keyType = (() => {
        try {
            const int = key.key_type().as_int();
            if (int?.is_positive())
                return parseInt(int.as_positive()?.to_str());
            return parseInt(int?.as_negative()?.to_str());
        }
        catch (_e) {
            throw new Error("Failed to retrieve Key Type.");
        }
    })();
    const publicKey = (() => {
        try {
            return _mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PublicKey.from_bytes(key.header(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.Label.new_int(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.Int.new_negative(_mod_js__WEBPACK_IMPORTED_MODULE_0__.M.BigNum.from_str("2"))))?.as_bytes());
        }
        catch (_e) {
            throw new Error("No public key found.");
        }
    })();
    const cose1Payload = (() => {
        try {
            return (0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.toHex)(cose1.payload());
        }
        catch (_e) {
            throw new Error("No payload found.");
        }
    })();
    const signature = _mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Ed25519Signature.from_bytes(cose1.signature());
    const data = cose1.signed_data(undefined, undefined).to_bytes();
    if (cose1Address !== addressHex)
        return false;
    if (keyHash !== publicKey.hash().to_hex())
        return false;
    if (cose1AlgorithmId !== keyAlgorithmId &&
        cose1AlgorithmId !== _mod_js__WEBPACK_IMPORTED_MODULE_0__.M.AlgorithmId.EdDSA) {
        return false;
    }
    if (keyCurve !== 6)
        return false;
    if (keyType !== 1)
        return false;
    if (cose1Payload !== payload)
        return false;
    return publicKey.verify(data, signature);
}

__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/misc/wallet.js":
/*!***********************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/misc/wallet.js ***!
  \***********************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "discoverOwnUsedTxKeyHashes": () => (/* binding */ discoverOwnUsedTxKeyHashes),
/* harmony export */   "walletFromSeed": () => (/* binding */ walletFromSeed)
/* harmony export */ });
/* harmony import */ var _mod_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../mod.js */ "./node_modules/lucid-cardano/esm/src/mod.js");
/* harmony import */ var _bip39_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./bip39.js */ "./node_modules/lucid-cardano/esm/src/misc/bip39.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_mod_js__WEBPACK_IMPORTED_MODULE_0__, _bip39_js__WEBPACK_IMPORTED_MODULE_1__]);
([_mod_js__WEBPACK_IMPORTED_MODULE_0__, _bip39_js__WEBPACK_IMPORTED_MODULE_1__] = __webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__);


function walletFromSeed(seed, options = { addressType: "Base", accountIndex: 0, network: "Mainnet" }) {
    function harden(num) {
        if (typeof num !== "number")
            throw new Error("Type number required here!");
        return 0x80000000 + num;
    }
    const entropy = (0,_bip39_js__WEBPACK_IMPORTED_MODULE_1__.mnemonicToEntropy)(seed);
    const rootKey = _mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Bip32PrivateKey.from_bip39_entropy((0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.fromHex)(entropy), options.password
        ? new TextEncoder().encode(options.password)
        : new Uint8Array());
    const accountKey = rootKey.derive(harden(1852))
        .derive(harden(1815))
        .derive(harden(options.accountIndex));
    const paymentKey = accountKey.derive(0).derive(0).to_raw_key();
    const stakeKey = accountKey.derive(2).derive(0).to_raw_key();
    const paymentKeyHash = paymentKey.to_public().hash();
    const stakeKeyHash = stakeKey.to_public().hash();
    const networkId = options.network === "Mainnet" ? 1 : 0;
    const address = options.addressType === "Base"
        ? _mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BaseAddress["new"](networkId, _mod_js__WEBPACK_IMPORTED_MODULE_0__.C.StakeCredential.from_keyhash(paymentKeyHash), _mod_js__WEBPACK_IMPORTED_MODULE_0__.C.StakeCredential.from_keyhash(stakeKeyHash)).to_address().to_bech32(undefined)
        : _mod_js__WEBPACK_IMPORTED_MODULE_0__.C.EnterpriseAddress["new"](networkId, _mod_js__WEBPACK_IMPORTED_MODULE_0__.C.StakeCredential.from_keyhash(paymentKeyHash)).to_address().to_bech32(undefined);
    const rewardAddress = options.addressType === "Base"
        ? _mod_js__WEBPACK_IMPORTED_MODULE_0__.C.RewardAddress["new"](networkId, _mod_js__WEBPACK_IMPORTED_MODULE_0__.C.StakeCredential.from_keyhash(stakeKeyHash)).to_address().to_bech32(undefined)
        : null;
    return {
        address,
        rewardAddress,
        paymentKey: paymentKey.to_bech32(),
        stakeKey: options.addressType === "Base" ? stakeKey.to_bech32() : null,
    };
}
function discoverOwnUsedTxKeyHashes(tx, ownKeyHashes, ownUtxos) {
    const usedKeyHashes = [];
    // key hashes from inputs
    const inputs = tx.body().inputs();
    for (let i = 0; i < inputs.len(); i++) {
        const input = inputs.get(i);
        const txHash = (0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.toHex)(input.transaction_id().to_bytes());
        const outputIndex = parseInt(input.index().to_str());
        const utxo = ownUtxos.find((utxo) => utxo.txHash === txHash && utxo.outputIndex === outputIndex);
        if (utxo) {
            const { paymentCredential } = (0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.getAddressDetails)(utxo.address);
            usedKeyHashes.push(paymentCredential?.hash);
        }
    }
    const txBody = tx.body();
    // key hashes from certificates
    function keyHashFromCert(txBody) {
        const certs = txBody.certs();
        if (!certs)
            return;
        for (let i = 0; i < certs.len(); i++) {
            const cert = certs.get(i);
            if (cert.kind() === 0) {
                const credential = cert.as_stake_registration()?.stake_credential();
                if (credential?.kind() === 0) {
                    // Key hash not needed for registration
                }
            }
            else if (cert.kind() === 1) {
                const credential = cert.as_stake_deregistration()?.stake_credential();
                if (credential?.kind() === 0) {
                    const keyHash = (0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.toHex)(credential.to_keyhash().to_bytes());
                    usedKeyHashes.push(keyHash);
                }
            }
            else if (cert.kind() === 2) {
                const credential = cert.as_stake_delegation()?.stake_credential();
                if (credential?.kind() === 0) {
                    const keyHash = (0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.toHex)(credential.to_keyhash().to_bytes());
                    usedKeyHashes.push(keyHash);
                }
            }
            else if (cert.kind() === 3) {
                const poolParams = cert
                    .as_pool_registration()?.pool_params();
                const owners = poolParams
                    ?.pool_owners();
                if (!owners)
                    break;
                for (let i = 0; i < owners.len(); i++) {
                    const keyHash = (0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.toHex)(owners.get(i).to_bytes());
                    usedKeyHashes.push(keyHash);
                }
                const operator = poolParams.operator().to_hex();
                usedKeyHashes.push(operator);
            }
            else if (cert.kind() === 4) {
                const operator = cert.as_pool_retirement().pool_keyhash().to_hex();
                usedKeyHashes.push(operator);
            }
            else if (cert.kind() === 6) {
                const instantRewards = cert
                    .as_move_instantaneous_rewards_cert()
                    ?.move_instantaneous_reward().as_to_stake_creds()
                    ?.keys();
                if (!instantRewards)
                    break;
                for (let i = 0; i < instantRewards.len(); i++) {
                    const credential = instantRewards.get(i);
                    if (credential.kind() === 0) {
                        const keyHash = (0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.toHex)(credential.to_keyhash().to_bytes());
                        usedKeyHashes.push(keyHash);
                    }
                }
            }
        }
    }
    if (txBody.certs())
        keyHashFromCert(txBody);
    // key hashes from withdrawals
    const withdrawals = txBody.withdrawals();
    function keyHashFromWithdrawal(withdrawals) {
        const rewardAddresses = withdrawals.keys();
        for (let i = 0; i < rewardAddresses.len(); i++) {
            const credential = rewardAddresses.get(i).payment_cred();
            if (credential.kind() === 0) {
                usedKeyHashes.push(credential.to_keyhash().to_hex());
            }
        }
    }
    if (withdrawals)
        keyHashFromWithdrawal(withdrawals);
    // key hashes from scripts
    const scripts = tx.witness_set().native_scripts();
    function keyHashFromScript(scripts) {
        for (let i = 0; i < scripts.len(); i++) {
            const script = scripts.get(i);
            if (script.kind() === 0) {
                const keyHash = (0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.toHex)(script.as_script_pubkey().addr_keyhash().to_bytes());
                usedKeyHashes.push(keyHash);
            }
            if (script.kind() === 1) {
                keyHashFromScript(script.as_script_all().native_scripts());
                return;
            }
            if (script.kind() === 2) {
                keyHashFromScript(script.as_script_any().native_scripts());
                return;
            }
            if (script.kind() === 3) {
                keyHashFromScript(script.as_script_n_of_k().native_scripts());
                return;
            }
        }
    }
    if (scripts)
        keyHashFromScript(scripts);
    // keyHashes from required signers
    const requiredSigners = txBody.required_signers();
    if (requiredSigners) {
        for (let i = 0; i < requiredSigners.len(); i++) {
            usedKeyHashes.push((0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.toHex)(requiredSigners.get(i).to_bytes()));
        }
    }
    // keyHashes from collateral
    const collateral = txBody.collateral();
    if (collateral) {
        for (let i = 0; i < collateral.len(); i++) {
            const input = collateral.get(i);
            const txHash = (0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.toHex)(input.transaction_id().to_bytes());
            const outputIndex = parseInt(input.index().to_str());
            const utxo = ownUtxos.find((utxo) => utxo.txHash === txHash && utxo.outputIndex === outputIndex);
            if (utxo) {
                const { paymentCredential } = (0,_mod_js__WEBPACK_IMPORTED_MODULE_0__.getAddressDetails)(utxo.address);
                usedKeyHashes.push(paymentCredential?.hash);
            }
        }
    }
    return usedKeyHashes.filter((k) => ownKeyHashes.includes(k));
}

__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/mod.js":
/*!***************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/mod.js ***!
  \***************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "Blockfrost": () => (/* reexport safe */ _provider_mod_js__WEBPACK_IMPORTED_MODULE_2__.Blockfrost),
/* harmony export */   "C": () => (/* reexport safe */ _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C),
/* harmony export */   "Constr": () => (/* reexport safe */ _plutus_mod_js__WEBPACK_IMPORTED_MODULE_5__.Constr),
/* harmony export */   "Data": () => (/* reexport safe */ _plutus_mod_js__WEBPACK_IMPORTED_MODULE_5__.Data),
/* harmony export */   "Emulator": () => (/* reexport safe */ _provider_mod_js__WEBPACK_IMPORTED_MODULE_2__.Emulator),
/* harmony export */   "Kupmios": () => (/* reexport safe */ _provider_mod_js__WEBPACK_IMPORTED_MODULE_2__.Kupmios),
/* harmony export */   "Lucid": () => (/* reexport safe */ _lucid_mod_js__WEBPACK_IMPORTED_MODULE_1__.Lucid),
/* harmony export */   "M": () => (/* reexport safe */ _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.M),
/* harmony export */   "MerkleTree": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.MerkleTree),
/* harmony export */   "PROTOCOL_PARAMETERS_DEFAULT": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.PROTOCOL_PARAMETERS_DEFAULT),
/* harmony export */   "SLOT_CONFIG_NETWORK": () => (/* reexport safe */ _plutus_mod_js__WEBPACK_IMPORTED_MODULE_5__.SLOT_CONFIG_NETWORK),
/* harmony export */   "Tx": () => (/* reexport safe */ _lucid_mod_js__WEBPACK_IMPORTED_MODULE_1__.Tx),
/* harmony export */   "TxComplete": () => (/* reexport safe */ _lucid_mod_js__WEBPACK_IMPORTED_MODULE_1__.TxComplete),
/* harmony export */   "TxSigned": () => (/* reexport safe */ _lucid_mod_js__WEBPACK_IMPORTED_MODULE_1__.TxSigned),
/* harmony export */   "Utils": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.Utils),
/* harmony export */   "applyDoubleCborEncoding": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.applyDoubleCborEncoding),
/* harmony export */   "applyParamsToScript": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.applyParamsToScript),
/* harmony export */   "assetsToValue": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.assetsToValue),
/* harmony export */   "combineHash": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.combineHash),
/* harmony export */   "concat": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.concat),
/* harmony export */   "coreToUtxo": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.coreToUtxo),
/* harmony export */   "createCostModels": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.createCostModels),
/* harmony export */   "datumJsonToCbor": () => (/* reexport safe */ _provider_mod_js__WEBPACK_IMPORTED_MODULE_2__.datumJsonToCbor),
/* harmony export */   "equals": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.equals),
/* harmony export */   "fromHex": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.fromHex),
/* harmony export */   "fromLabel": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.fromLabel),
/* harmony export */   "fromScriptRef": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.fromScriptRef),
/* harmony export */   "fromText": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.fromText),
/* harmony export */   "fromUnit": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.fromUnit),
/* harmony export */   "generatePrivateKey": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.generatePrivateKey),
/* harmony export */   "generateSeedPhrase": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.generateSeedPhrase),
/* harmony export */   "getAddressDetails": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.getAddressDetails),
/* harmony export */   "nativeScriptFromJson": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.nativeScriptFromJson),
/* harmony export */   "networkToId": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.networkToId),
/* harmony export */   "paymentCredentialOf": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.paymentCredentialOf),
/* harmony export */   "sha256": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.sha256),
/* harmony export */   "slotToBeginUnixTime": () => (/* reexport safe */ _plutus_mod_js__WEBPACK_IMPORTED_MODULE_5__.slotToBeginUnixTime),
/* harmony export */   "stakeCredentialOf": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.stakeCredentialOf),
/* harmony export */   "toHex": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.toHex),
/* harmony export */   "toLabel": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.toLabel),
/* harmony export */   "toPublicKey": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.toPublicKey),
/* harmony export */   "toScriptRef": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.toScriptRef),
/* harmony export */   "toText": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.toText),
/* harmony export */   "toUnit": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.toUnit),
/* harmony export */   "unixTimeToEnclosingSlot": () => (/* reexport safe */ _plutus_mod_js__WEBPACK_IMPORTED_MODULE_5__.unixTimeToEnclosingSlot),
/* harmony export */   "utxoToCore": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.utxoToCore),
/* harmony export */   "valueToAssets": () => (/* reexport safe */ _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__.valueToAssets)
/* harmony export */ });
/* harmony import */ var _core_mod_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./core/mod.js */ "./node_modules/lucid-cardano/esm/src/core/mod.js");
/* harmony import */ var _lucid_mod_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./lucid/mod.js */ "./node_modules/lucid-cardano/esm/src/lucid/mod.js");
/* harmony import */ var _provider_mod_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./provider/mod.js */ "./node_modules/lucid-cardano/esm/src/provider/mod.js");
/* harmony import */ var _types_mod_js__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ./types/mod.js */ "./node_modules/lucid-cardano/esm/src/types/mod.js");
/* harmony import */ var _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ./utils/mod.js */ "./node_modules/lucid-cardano/esm/src/utils/mod.js");
/* harmony import */ var _plutus_mod_js__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ./plutus/mod.js */ "./node_modules/lucid-cardano/esm/src/plutus/mod.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_core_mod_js__WEBPACK_IMPORTED_MODULE_0__, _lucid_mod_js__WEBPACK_IMPORTED_MODULE_1__, _provider_mod_js__WEBPACK_IMPORTED_MODULE_2__, _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__, _plutus_mod_js__WEBPACK_IMPORTED_MODULE_5__]);
([_core_mod_js__WEBPACK_IMPORTED_MODULE_0__, _lucid_mod_js__WEBPACK_IMPORTED_MODULE_1__, _provider_mod_js__WEBPACK_IMPORTED_MODULE_2__, _utils_mod_js__WEBPACK_IMPORTED_MODULE_4__, _plutus_mod_js__WEBPACK_IMPORTED_MODULE_5__] = __webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__);







__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/plutus/data.js":
/*!***********************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/plutus/data.js ***!
  \***********************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "Constr": () => (/* binding */ Constr),
/* harmony export */   "Data": () => (/* binding */ Data)
/* harmony export */ });
/* harmony import */ var _deps_deno_land_x_typebox_0_25_13_src_typebox_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../../deps/deno.land/x/typebox@0.25.13/src/typebox.js */ "./node_modules/lucid-cardano/esm/deps/deno.land/x/typebox@0.25.13/src/typebox.js");
/* harmony import */ var _core_mod_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../core/mod.js */ "./node_modules/lucid-cardano/esm/src/core/mod.js");
/* harmony import */ var _utils_utils_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../utils/utils.js */ "./node_modules/lucid-cardano/esm/src/utils/utils.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_core_mod_js__WEBPACK_IMPORTED_MODULE_1__, _utils_utils_js__WEBPACK_IMPORTED_MODULE_2__]);
([_core_mod_js__WEBPACK_IMPORTED_MODULE_1__, _utils_utils_js__WEBPACK_IMPORTED_MODULE_2__] = __webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__);



class Constr {
    constructor(index, fields) {
        Object.defineProperty(this, "index", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "fields", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        this.index = index;
        this.fields = fields;
    }
}
const Data = {
    // Types
    // Note: Recursive types are not supported (yet)
    Integer: function (options) {
        const integer = _deps_deno_land_x_typebox_0_25_13_src_typebox_js__WEBPACK_IMPORTED_MODULE_0__.Type.Unsafe({ dataType: "integer" });
        if (options) {
            Object.entries(options).forEach(([key, value]) => {
                integer[key] = value;
            });
        }
        return integer;
    },
    Bytes: function (options) {
        const bytes = _deps_deno_land_x_typebox_0_25_13_src_typebox_js__WEBPACK_IMPORTED_MODULE_0__.Type.Unsafe({ dataType: "bytes" });
        if (options) {
            Object.entries(options).forEach(([key, value]) => {
                bytes[key] = value;
            });
        }
        return bytes;
    },
    Boolean: function () {
        return _deps_deno_land_x_typebox_0_25_13_src_typebox_js__WEBPACK_IMPORTED_MODULE_0__.Type.Unsafe({
            anyOf: [
                {
                    title: "False",
                    dataType: "constructor",
                    index: 0,
                    fields: [],
                },
                {
                    title: "True",
                    dataType: "constructor",
                    index: 1,
                    fields: [],
                },
            ],
        });
    },
    Any: function () {
        return _deps_deno_land_x_typebox_0_25_13_src_typebox_js__WEBPACK_IMPORTED_MODULE_0__.Type.Unsafe({ description: "Any Data." });
    },
    Array: function (items, options) {
        const array = _deps_deno_land_x_typebox_0_25_13_src_typebox_js__WEBPACK_IMPORTED_MODULE_0__.Type.Array(items);
        replaceProperties(array, { dataType: "list", items });
        if (options) {
            Object.entries(options).forEach(([key, value]) => {
                array[key] = value;
            });
        }
        return array;
    },
    Map: function (keys, values, options) {
        const map = _deps_deno_land_x_typebox_0_25_13_src_typebox_js__WEBPACK_IMPORTED_MODULE_0__.Type.Unsafe({
            dataType: "map",
            keys,
            values,
        });
        if (options) {
            Object.entries(options).forEach(([key, value]) => {
                map[key] = value;
            });
        }
        return map;
    },
    /**
     * Object applies by default a PlutusData Constr with index 0.\
     * Set 'hasConstr' to false to serialize Object as PlutusData List.
     */
    Object: function (properties, options) {
        const object = _deps_deno_land_x_typebox_0_25_13_src_typebox_js__WEBPACK_IMPORTED_MODULE_0__.Type.Object(properties);
        replaceProperties(object, {
            anyOf: [{
                    dataType: "constructor",
                    index: 0,
                    fields: Object.entries(properties).map(([title, p]) => ({
                        ...p,
                        title,
                    })),
                }],
        });
        object.anyOf[0].hasConstr = typeof options?.hasConstr === "undefined" ||
            options.hasConstr;
        return object;
    },
    Enum: function (items) {
        const union = _deps_deno_land_x_typebox_0_25_13_src_typebox_js__WEBPACK_IMPORTED_MODULE_0__.Type.Union(items);
        replaceProperties(union, {
            anyOf: items.map((item, index) => item.anyOf[0].fields.length === 0
                ? ({
                    ...item.anyOf[0],
                    index,
                })
                : ({
                    dataType: "constructor",
                    title: (() => {
                        const title = item.anyOf[0].fields[0].title;
                        if (title.charAt(0) !==
                            title.charAt(0).toUpperCase()) {
                            throw new Error(`Enum '${title}' needs to start with an uppercase letter.`);
                        }
                        return item.anyOf[0].fields[0].title;
                    })(),
                    index,
                    fields: item.anyOf[0].fields[0].items,
                })),
        });
        return union;
    },
    /**
     * Tuple is by default a PlutusData List.\
     * Set 'hasConstr' to true to apply a PlutusData Constr with index 0.
     */
    Tuple: function (items, options) {
        const tuple = _deps_deno_land_x_typebox_0_25_13_src_typebox_js__WEBPACK_IMPORTED_MODULE_0__.Type.Tuple(items);
        replaceProperties(tuple, {
            dataType: "list",
            items,
        });
        if (options) {
            Object.entries(options).forEach(([key, value]) => {
                tuple[key] = value;
            });
        }
        return tuple;
    },
    Literal: function (title) {
        if (title.charAt(0) !== title.charAt(0).toUpperCase()) {
            throw new Error(`Enum '${title}' needs to start with an uppercase letter.`);
        }
        const literal = _deps_deno_land_x_typebox_0_25_13_src_typebox_js__WEBPACK_IMPORTED_MODULE_0__.Type.Literal(title);
        replaceProperties(literal, {
            anyOf: [{
                    dataType: "constructor",
                    title,
                    index: 0,
                    fields: [],
                }],
        });
        return literal;
    },
    Nullable: function (item) {
        return _deps_deno_land_x_typebox_0_25_13_src_typebox_js__WEBPACK_IMPORTED_MODULE_0__.Type.Unsafe({
            anyOf: [
                {
                    title: "Some",
                    description: "An optional value.",
                    dataType: "constructor",
                    index: 0,
                    fields: [
                        item,
                    ],
                },
                {
                    title: "None",
                    description: "Nothing.",
                    dataType: "constructor",
                    index: 1,
                    fields: [],
                },
            ],
        });
    },
    /**
     * Convert PlutusData to Cbor encoded data.\
     * Or apply a shape and convert the provided data struct to Cbor encoded data.
     */
    to,
    /** Convert Cbor encoded data to PlutusData */
    from,
    /**
     * Note Constr cannot be used here.\
     * Strings prefixed with '0x' are not UTF-8 encoded.
     */
    fromJson,
    /**
     * Note Constr cannot be used here, also only bytes/integers as Json keys.\
     */
    toJson,
    void: function () {
        return "d87980";
    },
    castFrom,
    castTo,
};
/**
 * Convert PlutusData to Cbor encoded data.\
 * Or apply a shape and convert the provided data struct to Cbor encoded data.
 */
function to(data, shape) {
    function serialize(data) {
        try {
            if (typeof data === "bigint") {
                return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusData.new_integer(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.BigInt.from_str(data.toString()));
            }
            else if (typeof data === "string") {
                return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusData.new_bytes((0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.fromHex)(data));
            }
            else if (data instanceof Constr) {
                const { index, fields } = data;
                const plutusList = _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusList["new"]();
                fields.forEach((field) => plutusList.add(serialize(field)));
                return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusData.new_constr_plutus_data(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ConstrPlutusData["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.BigNum.from_str(index.toString()), plutusList));
            }
            else if (data instanceof Array) {
                const plutusList = _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusList["new"]();
                data.forEach((arg) => plutusList.add(serialize(arg)));
                return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusData.new_list(plutusList);
            }
            else if (data instanceof Map) {
                const plutusMap = _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusMap["new"]();
                for (const [key, value] of data.entries()) {
                    plutusMap.insert(serialize(key), serialize(value));
                }
                return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusData.new_map(plutusMap);
            }
            throw new Error("Unsupported type");
        }
        catch (error) {
            throw new Error("Could not serialize the data: " + error);
        }
    }
    const d = shape ? castTo(data, shape) : data;
    return (0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.toHex)(serialize(d).to_bytes());
}
/**
 *  Convert Cbor encoded data to Data.\
 *  Or apply a shape and cast the cbor encoded data to a certain type.
 */
function from(raw, shape) {
    function deserialize(data) {
        if (data.kind() === 0) {
            const constr = data.as_constr_plutus_data();
            const l = constr.data();
            const desL = [];
            for (let i = 0; i < l.len(); i++) {
                desL.push(deserialize(l.get(i)));
            }
            return new Constr(parseInt(constr.alternative().to_str()), desL);
        }
        else if (data.kind() === 1) {
            const m = data.as_map();
            const desM = new Map();
            const keys = m.keys();
            for (let i = 0; i < keys.len(); i++) {
                desM.set(deserialize(keys.get(i)), deserialize(m.get(keys.get(i))));
            }
            return desM;
        }
        else if (data.kind() === 2) {
            const l = data.as_list();
            const desL = [];
            for (let i = 0; i < l.len(); i++) {
                desL.push(deserialize(l.get(i)));
            }
            return desL;
        }
        else if (data.kind() === 3) {
            return BigInt(data.as_integer().to_str());
        }
        else if (data.kind() === 4) {
            return (0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.toHex)(data.as_bytes());
        }
        throw new Error("Unsupported type");
    }
    const data = deserialize(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusData.from_bytes((0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.fromHex)(raw)));
    return shape ? castFrom(data, shape) : data;
}
/**
 * Note Constr cannot be used here.\
 * Strings prefixed with '0x' are not UTF-8 encoded.
 */
function fromJson(json) {
    function toData(json) {
        if (typeof json === "string") {
            return json.startsWith("0x")
                ? (0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.toHex)((0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.fromHex)(json.slice(2)))
                : (0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.fromText)(json);
        }
        if (typeof json === "number")
            return BigInt(json);
        if (typeof json === "bigint")
            return json;
        if (json instanceof Array)
            return json.map((v) => toData(v));
        if (json instanceof Object) {
            const tempMap = new Map();
            Object.entries(json).forEach(([key, value]) => {
                tempMap.set(toData(key), toData(value));
            });
            return tempMap;
        }
        throw new Error("Unsupported type");
    }
    return toData(json);
}
/**
 * Note Constr cannot be used here, also only bytes/integers as Json keys.\
 */
function toJson(plutusData) {
    function fromData(data) {
        if (typeof data === "bigint" ||
            typeof data === "number" ||
            (typeof data === "string" &&
                !isNaN(parseInt(data)) &&
                data.slice(-1) === "n")) {
            const bigint = typeof data === "string"
                ? BigInt(data.slice(0, -1))
                : data;
            return parseInt(bigint.toString());
        }
        if (typeof data === "string") {
            try {
                return new TextDecoder(undefined, { fatal: true }).decode((0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.fromHex)(data));
            }
            catch (_) {
                return "0x" + (0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.toHex)((0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.fromHex)(data));
            }
        }
        if (data instanceof Array)
            return data.map((v) => fromData(v));
        if (data instanceof Map) {
            const tempJson = {};
            data.forEach((value, key) => {
                const convertedKey = fromData(key);
                if (typeof convertedKey !== "string" &&
                    typeof convertedKey !== "number") {
                    throw new Error("Unsupported type (Note: Only bytes or integers can be keys of a JSON object)");
                }
                tempJson[convertedKey] = fromData(value);
            });
            return tempJson;
        }
        throw new Error("Unsupported type (Note: Constructor cannot be converted to JSON)");
    }
    return fromData(plutusData);
}
function castFrom(data, shape) {
    if (!shape)
        throw new Error("Could not type cast data.");
    const shapeType = (shape.anyOf ? "enum" : "") || shape.dataType;
    switch (shapeType) {
        case "integer": {
            if (typeof data !== "bigint") {
                throw new Error("Could not type cast to integer.");
            }
            integerConstraints(data, shape);
            return data;
        }
        case "bytes": {
            if (typeof data !== "string") {
                throw new Error("Could not type cast to bytes.");
            }
            bytesConstraints(data, shape);
            return data;
        }
        case "constructor": {
            if (data instanceof Constr && data.index === shape.index &&
                (shape.hasConstr || shape.hasConstr === undefined)) {
                const fields = {};
                if (shape.fields.length !== data.fields.length) {
                    throw new Error("Could not ype cast to object. Fields do not match.");
                }
                shape.fields.forEach((field, fieldIndex) => {
                    if ((/[A-Z]/.test(field.title[0]))) {
                        throw new Error("Could not type cast to object. Object properties need to start with a lowercase letter.");
                    }
                    fields[field.title] = castFrom(data.fields[fieldIndex], field);
                });
                return fields;
            }
            else if (data instanceof Array && !shape.hasConstr &&
                shape.hasConstr !== undefined) {
                const fields = {};
                if (shape.fields.length !== data.length) {
                    throw new Error("Could not ype cast to object. Fields do not match.");
                }
                shape.fields.forEach((field, fieldIndex) => {
                    if ((/[A-Z]/.test(field.title[0]))) {
                        throw new Error("Could not type cast to object. Object properties need to start with a lowercase letter.");
                    }
                    fields[field.title] = castFrom(data[fieldIndex], field);
                });
                return fields;
            }
            throw new Error("Could not type cast to object.");
        }
        case "enum": {
            // When enum has only one entry it's a single constructor/record object
            if (shape.anyOf.length === 1) {
                return castFrom(data, shape.anyOf[0]);
            }
            if (!(data instanceof Constr)) {
                throw new Error("Could not type cast to enum.");
            }
            const enumShape = shape.anyOf.find((entry) => entry.index === data.index);
            if (!enumShape || enumShape.fields.length !== data.fields.length) {
                throw new Error("Could not type cast to enum.");
            }
            if (isBoolean(shape)) {
                if (data.fields.length !== 0) {
                    throw new Error("Could not type cast to boolean.");
                }
                switch (data.index) {
                    case 0:
                        return false;
                    case 1:
                        return true;
                }
                throw new Error("Could not type cast to boolean.");
            }
            else if (isNullable(shape)) {
                switch (data.index) {
                    case 0: {
                        if (data.fields.length !== 1) {
                            throw new Error("Could not type cast to nullable object.");
                        }
                        return castFrom(data.fields[0], shape.anyOf[0].fields[0]);
                    }
                    case 1: {
                        if (data.fields.length !== 0) {
                            throw new Error("Could not type cast to nullable object.");
                        }
                        return null;
                    }
                }
                throw new Error("Could not type cast to nullable object.");
            }
            switch (enumShape.dataType) {
                case "constructor": {
                    if (enumShape.fields.length === 0) {
                        if (/[A-Z]/.test(enumShape.title[0])) {
                            return enumShape.title;
                        }
                        throw new Error("Could not type cast to enum.");
                    }
                    else {
                        if (!(/[A-Z]/.test(enumShape.title))) {
                            throw new Error("Could not type cast to enum. Enums need to start with an uppercase letter.");
                        }
                        if (enumShape.fields.length !== data.fields.length) {
                            throw new Error("Could not type cast to enum.");
                        }
                        return {
                            [enumShape.title]: enumShape.fields.map((field, index) => castFrom(data.fields[index], field)),
                        };
                    }
                }
            }
            throw new Error("Could not type cast to enum.");
        }
        case "list": {
            if (shape.items instanceof Array) {
                // tuple
                if (data instanceof Constr &&
                    data.index === 0 &&
                    shape.hasConstr) {
                    return data.fields.map((field, index) => castFrom(field, shape.items[index]));
                }
                else if (data instanceof Array && !shape.hasConstr) {
                    return data.map((field, index) => castFrom(field, shape.items[index]));
                }
                throw new Error("Could not type cast to tuple.");
            }
            else {
                // array
                if (!(data instanceof Array)) {
                    throw new Error("Could not type cast to array.");
                }
                listConstraints(data, shape);
                return data.map((field) => castFrom(field, shape.items));
            }
        }
        case "map": {
            if (!(data instanceof Map)) {
                throw new Error("Could not type cast to map.");
            }
            mapConstraints(data, shape);
            const map = new Map();
            for (const [key, value] of (data)
                .entries()) {
                map.set(castFrom(key, shape.keys), castFrom(value, shape.values));
            }
            return map;
        }
        case undefined: {
            return data;
        }
    }
    throw new Error("Could not type cast data.");
}
function castTo(struct, shape) {
    if (!shape)
        throw new Error("Could not type cast struct.");
    const shapeType = (shape.anyOf ? "enum" : "") || shape.dataType;
    switch (shapeType) {
        case "integer": {
            if (typeof struct !== "bigint") {
                throw new Error("Could not type cast to integer.");
            }
            integerConstraints(struct, shape);
            return struct;
        }
        case "bytes": {
            if (typeof struct !== "string") {
                throw new Error("Could not type cast to bytes.");
            }
            bytesConstraints(struct, shape);
            return struct;
        }
        case "constructor": {
            if (typeof struct !== "object" || struct === null) {
                throw new Error("Could not type cast to constructor.");
            }
            const fields = shape.fields.map((field) => castTo(struct[field.title], field));
            return (shape.hasConstr || shape.hasConstr === undefined)
                ? new Constr(shape.index, fields)
                : fields;
        }
        case "enum": {
            // When enum has only one entry it's a single constructor/record object
            if (shape.anyOf.length === 1) {
                return castTo(struct, shape.anyOf[0]);
            }
            if (isBoolean(shape)) {
                if (typeof struct !== "boolean") {
                    throw new Error("Could not type cast to boolean.");
                }
                return new Constr(struct ? 1 : 0, []);
            }
            else if (isNullable(shape)) {
                if (struct === null)
                    return new Constr(1, []);
                else {
                    const fields = shape.anyOf[0].fields;
                    if (fields.length !== 1) {
                        throw new Error("Could not type cast to nullable object.");
                    }
                    return new Constr(0, [
                        castTo(struct, fields[0]),
                    ]);
                }
            }
            switch (typeof struct) {
                case "string": {
                    if (!(/[A-Z]/.test(struct[0]))) {
                        throw new Error("Could not type cast to enum. Enum needs to start with an uppercase letter.");
                    }
                    const enumIndex = shape.anyOf.findIndex((s) => s.dataType === "constructor" &&
                        s.fields.length === 0 &&
                        s.title === struct);
                    if (enumIndex === -1)
                        throw new Error("Could not type cast to enum.");
                    return new Constr(enumIndex, []);
                }
                case "object": {
                    if (struct === null)
                        throw new Error("Could not type cast to enum.");
                    const structTitle = Object.keys(struct)[0];
                    if (!(/[A-Z]/.test(structTitle))) {
                        throw new Error("Could not type cast to enum. Enum needs to start with an uppercase letter.");
                    }
                    const enumEntry = shape.anyOf.find((s) => s.dataType === "constructor" &&
                        s.title === structTitle);
                    if (!enumEntry)
                        throw new Error("Could not type cast to enum.");
                    return new Constr(enumEntry.index, struct[structTitle].map((item, index) => castTo(item, enumEntry.fields[index])));
                }
            }
            throw new Error("Could not type cast to enum.");
        }
        case "list": {
            if (!(struct instanceof Array)) {
                throw new Error("Could not type cast to array/tuple.");
            }
            if (shape.items instanceof Array) {
                // tuple
                const fields = struct.map((item, index) => castTo(item, shape.items[index]));
                return shape.hasConstr ? new Constr(0, fields) : fields;
            }
            else {
                // array
                listConstraints(struct, shape);
                return struct.map((item) => castTo(item, shape.items));
            }
        }
        case "map": {
            if (!(struct instanceof Map)) {
                throw new Error("Could not type cast to map.");
            }
            mapConstraints(struct, shape);
            const map = new Map();
            for (const [key, value] of (struct)
                .entries()) {
                map.set(castTo(key, shape.keys), castTo(value, shape.values));
            }
            return map;
        }
        case undefined: {
            return struct;
        }
    }
    throw new Error("Could not type cast struct.");
}
function integerConstraints(integer, shape) {
    if (shape.minimum && integer < BigInt(shape.minimum)) {
        throw new Error(`Integer ${integer} is below the minimum ${shape.minimum}.`);
    }
    if (shape.maximum && integer > BigInt(shape.maximum)) {
        throw new Error(`Integer ${integer} is above the maxiumum ${shape.maximum}.`);
    }
    if (shape.exclusiveMinimum && integer <= BigInt(shape.exclusiveMinimum)) {
        throw new Error(`Integer ${integer} is below the exclusive minimum ${shape.exclusiveMinimum}.`);
    }
    if (shape.exclusiveMaximum && integer >= BigInt(shape.exclusiveMaximum)) {
        throw new Error(`Integer ${integer} is above the exclusive maximum ${shape.exclusiveMaximum}.`);
    }
}
function bytesConstraints(bytes, shape) {
    if (shape.enum && !shape.enum.some((keyword) => keyword === bytes))
        throw new Error(`None of the keywords match with '${bytes}'.`);
    if (shape.minLength && bytes.length / 2 < shape.minLength) {
        throw new Error(`Bytes need to have a length of at least ${shape.minLength} bytes.`);
    }
    if (shape.maxLength && bytes.length / 2 > shape.maxLength) {
        throw new Error(`Bytes can have a length of at most ${shape.minLength} bytes.`);
    }
}
function listConstraints(list, shape) {
    if (shape.minItems && list.length < shape.minItems) {
        throw new Error(`Array needs to contain at least ${shape.minItems} items.`);
    }
    if (shape.maxItems && list.length > shape.maxItems) {
        throw new Error(`Array can contain at most ${shape.maxItems} items.`);
    }
    if (shape.uniqueItems && (new Set(list)).size !== list.length) {
        // Note this only works for primitive types like string and bigint.
        throw new Error("Array constains duplicates.");
    }
}
function mapConstraints(map, shape) {
    if (shape.minItems && map.size < shape.minItems) {
        throw new Error(`Map needs to contain at least ${shape.minItems} items.`);
    }
    if (shape.maxItems && map.size > shape.maxItems) {
        throw new Error(`Map can contain at most ${shape.maxItems} items.`);
    }
}
function isBoolean(shape) {
    return shape.anyOf && shape.anyOf[0]?.title === "False" &&
        shape.anyOf[1]?.title === "True";
}
function isNullable(shape) {
    return shape.anyOf && shape.anyOf[0]?.title === "Some" &&
        shape.anyOf[1]?.title === "None";
}
function replaceProperties(object, properties) {
    Object.keys(object).forEach((key) => {
        delete object[key];
    });
    Object.assign(object, properties);
}

__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/plutus/mod.js":
/*!**********************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/plutus/mod.js ***!
  \**********************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "Constr": () => (/* reexport safe */ _data_js__WEBPACK_IMPORTED_MODULE_0__.Constr),
/* harmony export */   "Data": () => (/* reexport safe */ _data_js__WEBPACK_IMPORTED_MODULE_0__.Data),
/* harmony export */   "SLOT_CONFIG_NETWORK": () => (/* reexport safe */ _time_js__WEBPACK_IMPORTED_MODULE_1__.SLOT_CONFIG_NETWORK),
/* harmony export */   "slotToBeginUnixTime": () => (/* reexport safe */ _time_js__WEBPACK_IMPORTED_MODULE_1__.slotToBeginUnixTime),
/* harmony export */   "unixTimeToEnclosingSlot": () => (/* reexport safe */ _time_js__WEBPACK_IMPORTED_MODULE_1__.unixTimeToEnclosingSlot)
/* harmony export */ });
/* harmony import */ var _data_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./data.js */ "./node_modules/lucid-cardano/esm/src/plutus/data.js");
/* harmony import */ var _time_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./time.js */ "./node_modules/lucid-cardano/esm/src/plutus/time.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_data_js__WEBPACK_IMPORTED_MODULE_0__]);
_data_js__WEBPACK_IMPORTED_MODULE_0__ = (__webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__)[0];



__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/plutus/time.js":
/*!***********************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/plutus/time.js ***!
  \***********************************************************/
/***/ ((__unused_webpack___webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "SLOT_CONFIG_NETWORK": () => (/* binding */ SLOT_CONFIG_NETWORK),
/* harmony export */   "slotToBeginUnixTime": () => (/* binding */ slotToBeginUnixTime),
/* harmony export */   "unixTimeToEnclosingSlot": () => (/* binding */ unixTimeToEnclosingSlot)
/* harmony export */ });
const SLOT_CONFIG_NETWORK = {
    Mainnet: { zeroTime: 1596059091000, zeroSlot: 4492800, slotLength: 1000 },
    Preview: { zeroTime: 1666656000000, zeroSlot: 0, slotLength: 1000 },
    Preprod: {
        zeroTime: 1654041600000 + 1728000000,
        zeroSlot: 86400,
        slotLength: 1000,
    },
    /** Customizable slot config (Initialized with 0 values). */
    Custom: { zeroTime: 0, zeroSlot: 0, slotLength: 0 },
};
function slotToBeginUnixTime(slot, slotConfig) {
    const msAfterBegin = (slot - slotConfig.zeroSlot) * slotConfig.slotLength;
    return slotConfig.zeroTime + msAfterBegin;
}
// slotToBeginUnixTime and slotToEndUnixTime are identical when slotLength == 1. So we don't need to worry about this now.
// function slotToEndUnixTime(slot: Slot, slotConfig: SlotConfig): UnixTime {
//   return slotToBeginUnixTime(slot, slotConfig) + (slotConfig.slotLength - 1);
// }
function unixTimeToEnclosingSlot(unixTime, slotConfig) {
    const timePassed = unixTime - slotConfig.zeroTime;
    const slotsPassed = Math.floor(timePassed / slotConfig.slotLength);
    return slotsPassed + slotConfig.zeroSlot;
}


/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/provider/blockfrost.js":
/*!*******************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/provider/blockfrost.js ***!
  \*******************************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "Blockfrost": () => (/* binding */ Blockfrost),
/* harmony export */   "datumJsonToCbor": () => (/* binding */ datumJsonToCbor)
/* harmony export */ });
/* harmony import */ var _core_mod_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../core/mod.js */ "./node_modules/lucid-cardano/esm/src/core/mod.js");
/* harmony import */ var _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../utils/mod.js */ "./node_modules/lucid-cardano/esm/src/utils/mod.js");
/* harmony import */ var _package_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../../package.js */ "./node_modules/lucid-cardano/esm/package.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_core_mod_js__WEBPACK_IMPORTED_MODULE_0__, _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__]);
([_core_mod_js__WEBPACK_IMPORTED_MODULE_0__, _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__] = __webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__);



class Blockfrost {
    constructor(url, projectId) {
        Object.defineProperty(this, "url", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "projectId", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        this.url = url;
        this.projectId = projectId || "";
    }
    async getProtocolParameters() {
        const result = await fetch(`${this.url}/epochs/latest/parameters`, {
            headers: { project_id: this.projectId, lucid },
        }).then((res) => res.json());
        return {
            minFeeA: parseInt(result.min_fee_a),
            minFeeB: parseInt(result.min_fee_b),
            maxTxSize: parseInt(result.max_tx_size),
            maxValSize: parseInt(result.max_val_size),
            keyDeposit: BigInt(result.key_deposit),
            poolDeposit: BigInt(result.pool_deposit),
            priceMem: parseFloat(result.price_mem),
            priceStep: parseFloat(result.price_step),
            maxTxExMem: BigInt(result.max_tx_ex_mem),
            maxTxExSteps: BigInt(result.max_tx_ex_steps),
            coinsPerUtxoByte: BigInt(result.coins_per_utxo_size),
            collateralPercentage: parseInt(result.collateral_percent),
            maxCollateralInputs: parseInt(result.max_collateral_inputs),
            costModels: result.cost_models,
        };
    }
    async getUtxos(addressOrCredential) {
        const queryPredicate = (() => {
            if (typeof addressOrCredential === "string")
                return addressOrCredential;
            const credentialBech32 = addressOrCredential.type === "Key"
                ? _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Ed25519KeyHash.from_hex(addressOrCredential.hash).to_bech32("addr_vkh")
                : _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptHash.from_hex(addressOrCredential.hash).to_bech32("addr_vkh"); // should be 'script' (CIP-0005)
            return credentialBech32;
        })();
        let result = [];
        let page = 1;
        while (true) {
            const pageResult = await fetch(`${this.url}/addresses/${queryPredicate}/utxos?page=${page}`, { headers: { project_id: this.projectId, lucid } }).then((res) => res.json());
            if (pageResult.error) {
                if (pageResult.status_code === 404) {
                    return [];
                }
                else {
                    throw new Error("Could not fetch UTxOs from Blockfrost. Try again.");
                }
            }
            result = result.concat(pageResult);
            if (pageResult.length <= 0)
                break;
            page++;
        }
        return this.blockfrostUtxosToUtxos(result);
    }
    async getUtxosWithUnit(addressOrCredential, unit) {
        const queryPredicate = (() => {
            if (typeof addressOrCredential === "string")
                return addressOrCredential;
            const credentialBech32 = addressOrCredential.type === "Key"
                ? _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Ed25519KeyHash.from_hex(addressOrCredential.hash).to_bech32("addr_vkh")
                : _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptHash.from_hex(addressOrCredential.hash).to_bech32("addr_vkh"); // should be 'script' (CIP-0005)
            return credentialBech32;
        })();
        let result = [];
        let page = 1;
        while (true) {
            const pageResult = await fetch(`${this.url}/addresses/${queryPredicate}/utxos/${unit}?page=${page}`, { headers: { project_id: this.projectId, lucid } }).then((res) => res.json());
            if (pageResult.error) {
                if (pageResult.status_code === 404) {
                    return [];
                }
                else {
                    throw new Error("Could not fetch UTxOs from Blockfrost. Try again.");
                }
            }
            result = result.concat(pageResult);
            if (pageResult.length <= 0)
                break;
            page++;
        }
        return this.blockfrostUtxosToUtxos(result);
    }
    async getUtxoByUnit(unit) {
        const addresses = await fetch(`${this.url}/assets/${unit}/addresses?count=2`, { headers: { project_id: this.projectId, lucid } }).then((res) => res.json());
        if (!addresses || addresses.error) {
            throw new Error("Unit not found.");
        }
        if (addresses.length > 1) {
            throw new Error("Unit needs to be an NFT or only held by one address.");
        }
        const address = addresses[0].address;
        const utxos = await this.getUtxosWithUnit(address, unit);
        if (utxos.length > 1) {
            throw new Error("Unit needs to be an NFT or only held by one address.");
        }
        return utxos[0];
    }
    async getUtxosByOutRef(outRefs) {
        const queryHashes = [...new Set(outRefs.map((outRef) => outRef.txHash))];
        const utxos = await Promise.all(queryHashes.map(async (txHash) => {
            const result = await fetch(`${this.url}/txs/${txHash}/utxos`, { headers: { project_id: this.projectId, lucid } }).then((res) => res.json());
            if (!result || result.error) {
                return [];
            }
            const utxosResult = result.outputs.map((
            // deno-lint-ignore no-explicit-any
            r) => ({
                ...r,
                tx_hash: txHash,
            }));
            return this.blockfrostUtxosToUtxos(utxosResult);
        }));
        return utxos.reduce((acc, utxos) => acc.concat(utxos), []).filter((utxo) => outRefs.some((outRef) => utxo.txHash === outRef.txHash && utxo.outputIndex === outRef.outputIndex));
    }
    async getDelegation(rewardAddress) {
        const result = await fetch(`${this.url}/accounts/${rewardAddress}`, { headers: { project_id: this.projectId, lucid } }).then((res) => res.json());
        if (!result || result.error) {
            return { poolId: null, rewards: 0n };
        }
        return {
            poolId: result.pool_id || null,
            rewards: BigInt(result.withdrawable_amount),
        };
    }
    async getDatum(datumHash) {
        const datum = await fetch(`${this.url}/scripts/datum/${datumHash}/cbor`, {
            headers: { project_id: this.projectId, lucid },
        })
            .then((res) => res.json())
            .then((res) => res.cbor);
        if (!datum || datum.error) {
            throw new Error(`No datum found for datum hash: ${datumHash}`);
        }
        return datum;
    }
    awaitTx(txHash, checkInterval = 3000) {
        return new Promise((res) => {
            const confirmation = setInterval(async () => {
                const isConfirmed = await fetch(`${this.url}/txs/${txHash}`, {
                    headers: { project_id: this.projectId, lucid },
                }).then((res) => res.json());
                if (isConfirmed && !isConfirmed.error) {
                    clearInterval(confirmation);
                    res(true);
                    return;
                }
            }, checkInterval);
        });
    }
    async submitTx(tx) {
        const result = await fetch(`${this.url}/tx/submit`, {
            method: "POST",
            headers: {
                "Content-Type": "application/cbor",
                project_id: this.projectId,
                lucid,
            },
            body: (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(tx),
        }).then((res) => res.json());
        if (!result || result.error) {
            if (result?.status_code === 400)
                throw new Error(result.message);
            else
                throw new Error("Could not submit transaction.");
        }
        return result;
    }
    async blockfrostUtxosToUtxos(result) {
        return (await Promise.all(result.map(async (r) => ({
            txHash: r.tx_hash,
            outputIndex: r.output_index,
            assets: (() => {
                const a = {};
                r.amount.forEach((am) => {
                    a[am.unit] = BigInt(am.quantity);
                });
                return a;
            })(),
            address: r.address,
            datumHash: !r.inline_datum ? r.data_hash : undefined,
            datum: r.inline_datum,
            scriptRef: r.reference_script_hash &&
                (await (async () => {
                    const { type, } = await fetch(`${this.url}/scripts/${r.reference_script_hash}`, {
                        headers: { project_id: this.projectId, lucid },
                    }).then((res) => res.json());
                    // TODO: support native scripts
                    if (type === "Native" || type === "native") {
                        throw new Error("Native script ref not implemented!");
                    }
                    const { cbor: script } = await fetch(`${this.url}/scripts/${r.reference_script_hash}/cbor`, { headers: { project_id: this.projectId, lucid } }).then((res) => res.json());
                    return {
                        type: type === "plutusV1" ? "PlutusV1" : "PlutusV2",
                        script: (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.applyDoubleCborEncoding)(script),
                    };
                })()),
        }))));
    }
}
/**
 * This function is temporarily needed only, until Blockfrost returns the datum natively in Cbor.
 * The conversion is ambigious, that's why it's better to get the datum directly in Cbor.
 */
function datumJsonToCbor(json) {
    const convert = (json) => {
        if (!isNaN(json.int)) {
            return _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusData.new_integer(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigInt.from_str(json.int.toString()));
        }
        else if (json.bytes || !isNaN(Number(json.bytes))) {
            return _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusData.new_bytes((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(json.bytes));
        }
        else if (json.map) {
            const m = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusMap["new"]();
            json.map.forEach(({ k, v }) => {
                m.insert(convert(k), convert(v));
            });
            return _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusData.new_map(m);
        }
        else if (json.list) {
            const l = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusList["new"]();
            json.list.forEach((v) => {
                l.add(convert(v));
            });
            return _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusData.new_list(l);
        }
        else if (!isNaN(json.constructor)) {
            const l = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusList["new"]();
            json.fields.forEach((v) => {
                l.add(convert(v));
            });
            return _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusData.new_constr_plutus_data(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.ConstrPlutusData["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(json.constructor.toString()), l));
        }
        throw new Error("Unsupported type");
    };
    return (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.toHex)(convert(json).to_bytes());
}
const lucid = _package_js__WEBPACK_IMPORTED_MODULE_2__["default"].version; // Lucid version

__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/provider/emulator.js":
/*!*****************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/provider/emulator.js ***!
  \*****************************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "Emulator": () => (/* binding */ Emulator)
/* harmony export */ });
/* harmony import */ var _core_core_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../core/core.js */ "./node_modules/lucid-cardano/esm/src/core/core.js");
/* harmony import */ var _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../utils/mod.js */ "./node_modules/lucid-cardano/esm/src/utils/mod.js");
/* harmony import */ var _utils_utils_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../utils/utils.js */ "./node_modules/lucid-cardano/esm/src/utils/utils.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_core_core_js__WEBPACK_IMPORTED_MODULE_0__, _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__, _utils_utils_js__WEBPACK_IMPORTED_MODULE_2__]);
([_core_core_js__WEBPACK_IMPORTED_MODULE_0__, _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__, _utils_utils_js__WEBPACK_IMPORTED_MODULE_2__] = __webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__);



class Emulator {
    constructor(accounts, protocolParameters = _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.PROTOCOL_PARAMETERS_DEFAULT) {
        Object.defineProperty(this, "ledger", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "mempool", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: {}
        });
        /**
         * Only stake key registrations/delegations and rewards are tracked.
         * Other certificates are not tracked.
         */
        Object.defineProperty(this, "chain", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: {}
        });
        Object.defineProperty(this, "blockHeight", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "slot", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "time", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "protocolParameters", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "datumTable", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: {}
        });
        const GENESIS_HASH = "00".repeat(32);
        this.blockHeight = 0;
        this.slot = 0;
        this.time = Date.now();
        this.ledger = {};
        accounts.forEach(({ address, assets }, index) => {
            this.ledger[GENESIS_HASH + index] = {
                utxo: {
                    txHash: GENESIS_HASH,
                    outputIndex: index,
                    address,
                    assets,
                },
                spent: false,
            };
        });
        this.protocolParameters = protocolParameters;
    }
    now() {
        return this.time;
    }
    awaitSlot(length = 1) {
        this.slot += length;
        this.time += length * 1000;
        const currentHeight = this.blockHeight;
        this.blockHeight = Math.floor(this.slot / 20);
        if (this.blockHeight > currentHeight) {
            for (const [outRef, { utxo, spent }] of Object.entries(this.mempool)) {
                this.ledger[outRef] = { utxo, spent };
            }
            for (const [outRef, { spent }] of Object.entries(this.ledger)) {
                if (spent)
                    delete this.ledger[outRef];
            }
            this.mempool = {};
        }
    }
    awaitBlock(height = 1) {
        this.blockHeight += height;
        this.slot += height * 20;
        this.time += height * 20 * 1000;
        for (const [outRef, { utxo, spent }] of Object.entries(this.mempool)) {
            this.ledger[outRef] = { utxo, spent };
        }
        for (const [outRef, { spent }] of Object.entries(this.ledger)) {
            if (spent)
                delete this.ledger[outRef];
        }
        this.mempool = {};
    }
    getUtxos(addressOrCredential) {
        const utxos = Object.values(this.ledger).flatMap(({ utxo }) => {
            if (typeof addressOrCredential === "string") {
                return addressOrCredential === utxo.address ? utxo : [];
            }
            else {
                const { paymentCredential } = (0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.getAddressDetails)(utxo.address);
                return paymentCredential?.hash === addressOrCredential.hash ? utxo : [];
            }
        });
        return Promise.resolve(utxos);
    }
    getProtocolParameters() {
        return Promise.resolve(this.protocolParameters);
    }
    getDatum(datumHash) {
        return Promise.resolve(this.datumTable[datumHash]);
    }
    getUtxosWithUnit(addressOrCredential, unit) {
        const utxos = Object.values(this.ledger).flatMap(({ utxo }) => {
            if (typeof addressOrCredential === "string") {
                return addressOrCredential === utxo.address && utxo.assets[unit] > 0n
                    ? utxo
                    : [];
            }
            else {
                const { paymentCredential } = (0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.getAddressDetails)(utxo.address);
                return paymentCredential?.hash === addressOrCredential.hash &&
                    utxo.assets[unit] > 0n
                    ? utxo
                    : [];
            }
        });
        return Promise.resolve(utxos);
    }
    getUtxosByOutRef(outRefs) {
        return Promise.resolve(outRefs.flatMap((outRef) => this.ledger[outRef.txHash + outRef.outputIndex]?.utxo || []));
    }
    getUtxoByUnit(unit) {
        const utxos = Object.values(this.ledger).flatMap(({ utxo }) => utxo.assets[unit] > 0n ? utxo : []);
        if (utxos.length > 1) {
            throw new Error("Unit needs to be an NFT or only held by one address.");
        }
        return Promise.resolve(utxos[0]);
    }
    getDelegation(rewardAddress) {
        return Promise.resolve({
            poolId: this.chain[rewardAddress]?.delegation?.poolId || null,
            rewards: this.chain[rewardAddress]?.delegation?.rewards || 0n,
        });
    }
    awaitTx(txHash) {
        if (this.mempool[txHash + 0]) {
            this.awaitBlock();
            return Promise.resolve(true);
        }
        return Promise.resolve(true);
    }
    /**
     * Emulates the behaviour of the reward distribution at epoch boundaries.
     * Stake keys need to be registered and delegated like on a real chain in order to receive rewards.
     */
    distributeRewards(rewards) {
        for (const [rewardAddress, { registeredStake, delegation }] of Object.entries(this.chain)) {
            if (registeredStake && delegation.poolId) {
                this.chain[rewardAddress] = {
                    registeredStake,
                    delegation: {
                        poolId: delegation.poolId,
                        rewards: delegation.rewards += rewards,
                    },
                };
            }
        }
        this.awaitBlock();
    }
    submitTx(tx) {
        /*
            Checks that are already handled by the transaction builder:
              - Fee calculation
              - Phase 2 evaluation
              - Input value == Output value (including mint value)
              - Min ada requirement
              - Stake key registration deposit amount
              - Collateral
    
            Checks that need to be done:
              - Verify witnesses
              - Correct count of scripts and vkeys
              - Stake key registration
              - Withdrawals
              - Validity interval
         */
        const desTx = _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.Transaction.from_bytes((0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.fromHex)(tx));
        const body = desTx.body();
        const witnesses = desTx.witness_set();
        const datums = witnesses.plutus_data();
        const txHash = _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.hash_transaction(body).to_hex();
        // Validity interval
        // Lower bound is inclusive?
        // Upper bound is inclusive?
        const lowerBound = body.validity_start_interval()
            ? parseInt(body.validity_start_interval().to_str())
            : null;
        const upperBound = body.ttl() ? parseInt(body.ttl().to_str()) : null;
        if (Number.isInteger(lowerBound) && this.slot < lowerBound) {
            throw new Error(`Lower bound (${lowerBound}) not in slot range (${this.slot}).`);
        }
        if (Number.isInteger(upperBound) && this.slot > upperBound) {
            throw new Error(`Upper bound (${upperBound}) not in slot range (${this.slot}).`);
        }
        // Datums in witness set
        const datumTable = (() => {
            const table = {};
            for (let i = 0; i < (datums?.len() || 0); i++) {
                const datum = datums.get(i);
                const datumHash = _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.hash_plutus_data(datum).to_hex();
                table[datumHash] = (0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.toHex)(datum.to_bytes());
            }
            return table;
        })();
        const consumedHashes = new Set();
        // Witness keys
        const keyHashes = (() => {
            const keyHashes = [];
            for (let i = 0; i < (witnesses.vkeys()?.len() || 0); i++) {
                const witness = witnesses.vkeys().get(i);
                const publicKey = witness.vkey().public_key();
                const keyHash = publicKey.hash().to_hex();
                if (!publicKey.verify((0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.fromHex)(txHash), witness.signature())) {
                    throw new Error(`Invalid vkey witness. Key hash: ${keyHash}`);
                }
                keyHashes.push(keyHash);
            }
            return keyHashes;
        })();
        // We only need this to verify native scripts. The check happens in the CML.
        const edKeyHashes = _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.Ed25519KeyHashes["new"]();
        keyHashes.forEach((keyHash) => edKeyHashes.add(_core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.Ed25519KeyHash.from_hex(keyHash)));
        const nativeHashes = (() => {
            const scriptHashes = [];
            for (let i = 0; i < (witnesses.native_scripts()?.len() || 0); i++) {
                const witness = witnesses.native_scripts().get(i);
                const scriptHash = witness.hash(_core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptHashNamespace.NativeScript)
                    .to_hex();
                if (!witness.verify(Number.isInteger(lowerBound)
                    ? _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(lowerBound.toString())
                    : undefined, Number.isInteger(upperBound)
                    ? _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(upperBound.toString())
                    : undefined, edKeyHashes)) {
                    throw new Error(`Invalid native script witness. Script hash: ${scriptHash}`);
                }
                for (let i = 0; i < witness.get_required_signers().len(); i++) {
                    const keyHash = witness.get_required_signers().get(i).to_hex();
                    consumedHashes.add(keyHash);
                }
                scriptHashes.push(scriptHash);
            }
            return scriptHashes;
        })();
        const nativeHashesOptional = {};
        const plutusHashesOptional = [];
        const plutusHashes = (() => {
            const scriptHashes = [];
            for (let i = 0; i < (witnesses.plutus_scripts()?.len() || 0); i++) {
                const script = witnesses.plutus_scripts().get(i);
                const scriptHash = script.hash(_core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptHashNamespace.PlutusV1)
                    .to_hex();
                scriptHashes.push(scriptHash);
            }
            for (let i = 0; i < (witnesses.plutus_v2_scripts()?.len() || 0); i++) {
                const script = witnesses.plutus_v2_scripts().get(i);
                const scriptHash = script.hash(_core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptHashNamespace.PlutusV2)
                    .to_hex();
                scriptHashes.push(scriptHash);
            }
            return scriptHashes;
        })();
        const inputs = body.inputs();
        inputs.sort();
        const resolvedInputs = [];
        // Check existence of inputs and look for script refs.
        for (let i = 0; i < inputs.len(); i++) {
            const input = inputs.get(i);
            const outRef = input.transaction_id().to_hex() + input.index().to_str();
            const entryLedger = this.ledger[outRef];
            const { entry, type } = !entryLedger
                ? { entry: this.mempool[outRef], type: "Mempool" }
                : { entry: entryLedger, type: "Ledger" };
            if (!entry || entry.spent) {
                throw new Error(`Could not spend UTxO: ${JSON.stringify({
                    txHash: entry?.utxo.txHash,
                    outputIndex: entry?.utxo.outputIndex,
                })}\nIt does not exist or was already spent.`);
            }
            const scriptRef = entry.utxo.scriptRef;
            if (scriptRef) {
                switch (scriptRef.type) {
                    case "Native": {
                        const script = _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.NativeScript.from_bytes((0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.fromHex)(scriptRef.script));
                        nativeHashesOptional[script.hash(_core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptHashNamespace.NativeScript).to_hex()] = script;
                        break;
                    }
                    case "PlutusV1": {
                        const script = _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusScript.from_bytes((0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.fromHex)(scriptRef.script));
                        plutusHashesOptional.push(script.hash(_core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptHashNamespace.PlutusV1).to_hex());
                        break;
                    }
                    case "PlutusV2": {
                        const script = _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusScript.from_bytes((0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.fromHex)(scriptRef.script));
                        plutusHashesOptional.push(script.hash(_core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptHashNamespace.PlutusV2).to_hex());
                        break;
                    }
                }
            }
            if (entry.utxo.datumHash)
                consumedHashes.add(entry.utxo.datumHash);
            resolvedInputs.push({ entry, type });
        }
        // Check existence of reference inputs and look for script refs.
        for (let i = 0; i < (body.reference_inputs()?.len() || 0); i++) {
            const input = body.reference_inputs().get(i);
            const outRef = input.transaction_id().to_hex() + input.index().to_str();
            const entry = this.ledger[outRef] || this.mempool[outRef];
            if (!entry || entry.spent) {
                throw new Error(`Could not read UTxO: ${JSON.stringify({
                    txHash: entry?.utxo.txHash,
                    outputIndex: entry?.utxo.outputIndex,
                })}\nIt does not exist or was already spent.`);
            }
            const scriptRef = entry.utxo.scriptRef;
            if (scriptRef) {
                switch (scriptRef.type) {
                    case "Native": {
                        const script = _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.NativeScript.from_bytes((0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.fromHex)(scriptRef.script));
                        nativeHashesOptional[script.hash(_core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptHashNamespace.NativeScript).to_hex()] = script;
                        break;
                    }
                    case "PlutusV1": {
                        const script = _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusScript.from_bytes((0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.fromHex)(scriptRef.script));
                        plutusHashesOptional.push(script.hash(_core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptHashNamespace.PlutusV1).to_hex());
                        break;
                    }
                    case "PlutusV2": {
                        const script = _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusScript.from_bytes((0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.fromHex)(scriptRef.script));
                        plutusHashesOptional.push(script.hash(_core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.ScriptHashNamespace.PlutusV2).to_hex());
                        break;
                    }
                }
            }
            if (entry.utxo.datumHash)
                consumedHashes.add(entry.utxo.datumHash);
        }
        const redeemers = (() => {
            const tagMap = {
                0: "Spend",
                1: "Mint",
                2: "Cert",
                3: "Reward",
            };
            const collected = [];
            for (let i = 0; i < (witnesses.redeemers()?.len() || 0); i++) {
                const redeemer = witnesses.redeemers().get(i);
                collected.push({
                    tag: tagMap[redeemer.tag().kind()],
                    index: parseInt(redeemer.index().to_str()),
                });
            }
            return collected;
        })();
        function checkAndConsumeHash(credential, tag, index) {
            switch (credential.type) {
                case "Key": {
                    if (!keyHashes.includes(credential.hash)) {
                        throw new Error(`Missing vkey witness. Key hash: ${credential.hash}`);
                    }
                    consumedHashes.add(credential.hash);
                    break;
                }
                case "Script": {
                    if (nativeHashes.includes(credential.hash)) {
                        consumedHashes.add(credential.hash);
                        break;
                    }
                    else if (nativeHashesOptional[credential.hash]) {
                        if (!nativeHashesOptional[credential.hash].verify(Number.isInteger(lowerBound)
                            ? _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(lowerBound.toString())
                            : undefined, Number.isInteger(upperBound)
                            ? _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(upperBound.toString())
                            : undefined, edKeyHashes)) {
                            throw new Error(`Invalid native script witness. Script hash: ${credential.hash}`);
                        }
                        break;
                    }
                    else if (plutusHashes.includes(credential.hash) ||
                        plutusHashesOptional.includes(credential.hash)) {
                        if (redeemers.find((redeemer) => redeemer.tag === tag && redeemer.index === index)) {
                            consumedHashes.add(credential.hash);
                            break;
                        }
                    }
                    throw new Error(`Missing script witness. Script hash: ${credential.hash}`);
                }
            }
        }
        // Check collateral inputs
        for (let i = 0; i < (body.collateral()?.len() || 0); i++) {
            const input = body.collateral().get(i);
            const outRef = input.transaction_id().to_hex() + input.index().to_str();
            const entry = this.ledger[outRef] || this.mempool[outRef];
            if (!entry || entry.spent) {
                throw new Error(`Could not read UTxO: ${JSON.stringify({
                    txHash: entry?.utxo.txHash,
                    outputIndex: entry?.utxo.outputIndex,
                })}\nIt does not exist or was already spent.`);
            }
            const { paymentCredential } = (0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.getAddressDetails)(entry.utxo.address);
            if (paymentCredential?.type === "Script") {
                throw new Error("Collateral inputs can only contain vkeys.");
            }
            checkAndConsumeHash(paymentCredential, null, null);
        }
        // Check required signers
        for (let i = 0; i < (body.required_signers()?.len() || 0); i++) {
            const signer = body.required_signers().get(i);
            checkAndConsumeHash({ type: "Key", hash: signer.to_hex() }, null, null);
        }
        // Check mint witnesses
        for (let index = 0; index < (body.mint()?.keys().len() || 0); index++) {
            const policyId = body.mint().keys().get(index).to_hex();
            checkAndConsumeHash({ type: "Script", hash: policyId }, "Mint", index);
        }
        // Check withdrawal witnesses
        const withdrawalRequests = [];
        for (let index = 0; index < (body.withdrawals()?.keys().len() || 0); index++) {
            const rawAddress = body.withdrawals().keys().get(index);
            const withdrawal = BigInt(body.withdrawals().get(rawAddress).to_str());
            const rewardAddress = rawAddress.to_address().to_bech32(undefined);
            const { stakeCredential } = (0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.getAddressDetails)(rewardAddress);
            checkAndConsumeHash(stakeCredential, "Reward", index);
            if (this.chain[rewardAddress]?.delegation.rewards !== withdrawal) {
                throw new Error("Withdrawal amount doesn't match actual reward balance.");
            }
            withdrawalRequests.push({ rewardAddress, withdrawal });
        }
        // Check cert witnesses
        const certRequests = [];
        for (let index = 0; index < (body.certs()?.len() || 0); index++) {
            /*
              Checking only:
              1. Stake registration
              2. Stake deregistration
              3. Stake delegation
      
              All other certificate types are not checked and considered valid.
            */
            const cert = body.certs().get(index);
            switch (cert.kind()) {
                case 0: {
                    const registration = cert.as_stake_registration();
                    const rewardAddress = _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.RewardAddress["new"](_core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.NetworkInfo.testnet().network_id(), registration.stake_credential()).to_address().to_bech32(undefined);
                    if (this.chain[rewardAddress]?.registeredStake) {
                        throw new Error(`Stake key is already registered. Reward address: ${rewardAddress}`);
                    }
                    certRequests.push({ type: "Registration", rewardAddress });
                    break;
                }
                case 1: {
                    const deregistration = cert.as_stake_deregistration();
                    const rewardAddress = _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.RewardAddress["new"](_core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.NetworkInfo.testnet().network_id(), deregistration.stake_credential()).to_address().to_bech32(undefined);
                    const { stakeCredential } = (0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.getAddressDetails)(rewardAddress);
                    checkAndConsumeHash(stakeCredential, "Cert", index);
                    if (!this.chain[rewardAddress]?.registeredStake) {
                        throw new Error(`Stake key is already deregistered. Reward address: ${rewardAddress}`);
                    }
                    certRequests.push({ type: "Deregistration", rewardAddress });
                    break;
                }
                case 2: {
                    const delegation = cert.as_stake_delegation();
                    const rewardAddress = _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.RewardAddress["new"](_core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.NetworkInfo.testnet().network_id(), delegation.stake_credential()).to_address().to_bech32(undefined);
                    const poolId = delegation.pool_keyhash().to_bech32("pool");
                    const { stakeCredential } = (0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.getAddressDetails)(rewardAddress);
                    checkAndConsumeHash(stakeCredential, "Cert", index);
                    if (!this.chain[rewardAddress]?.registeredStake &&
                        !certRequests.find((request) => request.type === "Registration" &&
                            request.rewardAddress === rewardAddress)) {
                        throw new Error(`Stake key is not registered. Reward address: ${rewardAddress}`);
                    }
                    certRequests.push({ type: "Delegation", rewardAddress, poolId });
                    break;
                }
            }
        }
        // Check input witnesses
        resolvedInputs.forEach(({ entry: { utxo } }, index) => {
            const { paymentCredential } = (0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.getAddressDetails)(utxo.address);
            checkAndConsumeHash(paymentCredential, "Spend", index);
        });
        // Create outputs and consume datum hashes
        const outputs = (() => {
            const collected = [];
            for (let i = 0; i < body.outputs().len(); i++) {
                const output = body.outputs().get(i);
                const unspentOutput = _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionUnspentOutput["new"](_core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionInput["new"](_core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.TransactionHash.from_hex(txHash), _core_core_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(i.toString())), output);
                const utxo = (0,_utils_utils_js__WEBPACK_IMPORTED_MODULE_2__.coreToUtxo)(unspentOutput);
                if (utxo.datumHash)
                    consumedHashes.add(utxo.datumHash);
                collected.push({
                    utxo,
                    spent: false,
                });
            }
            return collected;
        })();
        // Check consumed witnesses
        const [extraKeyHash] = keyHashes.filter((keyHash) => !consumedHashes.has(keyHash));
        if (extraKeyHash) {
            throw new Error(`Extraneous vkey witness. Key hash: ${extraKeyHash}`);
        }
        const [extraNativeHash] = nativeHashes.filter((scriptHash) => !consumedHashes.has(scriptHash));
        if (extraNativeHash) {
            throw new Error(`Extraneous native script. Script hash: ${extraNativeHash}`);
        }
        const [extraPlutusHash] = plutusHashes.filter((scriptHash) => !consumedHashes.has(scriptHash));
        if (extraPlutusHash) {
            throw new Error(`Extraneous plutus script. Script hash: ${extraPlutusHash}`);
        }
        const [extraDatumHash] = Object.keys(datumTable).filter((datumHash) => !consumedHashes.has(datumHash));
        if (extraDatumHash) {
            throw new Error(`Extraneous plutus data. Datum hash: ${extraDatumHash}`);
        }
        // Apply transitions
        resolvedInputs.forEach(({ entry, type }) => {
            const outRef = entry.utxo.txHash + entry.utxo.outputIndex;
            entry.spent = true;
            if (type === "Ledger")
                this.ledger[outRef] = entry;
            else if (type === "Mempool")
                this.mempool[outRef] = entry;
        });
        withdrawalRequests.forEach(({ rewardAddress, withdrawal }) => {
            this.chain[rewardAddress].delegation.rewards -= withdrawal;
        });
        certRequests.forEach(({ type, rewardAddress, poolId }) => {
            switch (type) {
                case "Registration": {
                    if (this.chain[rewardAddress]) {
                        this.chain[rewardAddress].registeredStake = true;
                    }
                    else {
                        this.chain[rewardAddress] = {
                            registeredStake: true,
                            delegation: { poolId: null, rewards: 0n },
                        };
                    }
                    break;
                }
                case "Deregistration": {
                    this.chain[rewardAddress].registeredStake = false;
                    this.chain[rewardAddress].delegation.poolId = null;
                    break;
                }
                case "Delegation": {
                    this.chain[rewardAddress].delegation.poolId = poolId;
                }
            }
        });
        outputs.forEach(({ utxo, spent }) => {
            this.mempool[utxo.txHash + utxo.outputIndex] = {
                utxo,
                spent,
            };
        });
        for (const [datumHash, datum] of Object.entries(datumTable)) {
            this.datumTable[datumHash] = datum;
        }
        return Promise.resolve(txHash);
    }
    log() {
        function getRandomColor(unit) {
            const seed = unit === "lovelace" ? "1" : unit;
            // Convert the seed string to a number
            let num = 0;
            for (let i = 0; i < seed.length; i++) {
                num += seed.charCodeAt(i);
            }
            // Generate a color based on the seed number
            const r = (num * 123) % 256;
            const g = (num * 321) % 256;
            const b = (num * 213) % 256;
            // Return the color as a hex string
            return "#" + ((1 << 24) + (r << 16) + (g << 8) + b).toString(16).slice(1);
        }
        const totalBalances = {};
        const balances = {};
        for (const { utxo } of Object.values(this.ledger)) {
            for (const [unit, quantity] of Object.entries(utxo.assets)) {
                if (!balances[utxo.address]) {
                    balances[utxo.address] = { [unit]: quantity };
                }
                else if (!balances[utxo.address]?.[unit]) {
                    balances[utxo.address][unit] = quantity;
                }
                else {
                    balances[utxo.address][unit] += quantity;
                }
                if (!totalBalances[unit]) {
                    totalBalances[unit] = quantity;
                }
                else {
                    totalBalances[unit] += quantity;
                }
            }
        }
        console.log("\n%cBlockchain state", "color:purple");
        console.log(`
    Block height:   %c${this.blockHeight}%c
    Slot:           %c${this.slot}%c
    Unix time:      %c${this.time}
  `, "color:yellow", "color:white", "color:yellow", "color:white", "color:yellow");
        console.log("\n");
        for (const [address, assets] of Object.entries(balances)) {
            console.log(`Address: %c${address}`, "color:blue", "\n");
            for (const [unit, quantity] of Object.entries(assets)) {
                const barLength = Math.max(Math.floor(60 * (Number(quantity) / Number(totalBalances[unit]))), 1);
                console.log(`%c${"\u2586".repeat(barLength) + " ".repeat(60 - barLength)}`, `color: ${getRandomColor(unit)}`, "", `${unit}:`, quantity, "");
            }
            console.log(`\n${"\u2581".repeat(60)}\n`);
        }
    }
}

__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/provider/kupmios.js":
/*!****************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/provider/kupmios.js ***!
  \****************************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "Kupmios": () => (/* binding */ Kupmios)
/* harmony export */ });
/* harmony import */ var _core_mod_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../core/mod.js */ "./node_modules/lucid-cardano/esm/src/core/mod.js");
/* harmony import */ var _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../utils/mod.js */ "./node_modules/lucid-cardano/esm/src/utils/mod.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_core_mod_js__WEBPACK_IMPORTED_MODULE_0__, _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__]);
([_core_mod_js__WEBPACK_IMPORTED_MODULE_0__, _utils_mod_js__WEBPACK_IMPORTED_MODULE_1__] = __webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__);


class Kupmios {
    /**
     * @param kupoUrl: http(s)://localhost:1442
     * @param ogmiosUrl: ws(s)://localhost:1337
     */
    constructor(kupoUrl, ogmiosUrl) {
        Object.defineProperty(this, "kupoUrl", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        Object.defineProperty(this, "ogmiosUrl", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        this.kupoUrl = kupoUrl;
        this.ogmiosUrl = ogmiosUrl;
    }
    async getProtocolParameters() {
        const client = await this.ogmiosWsp("Query", {
            query: "currentProtocolParameters",
        });
        return new Promise((res, rej) => {
            client.addEventListener("message", (msg) => {
                try {
                    const { result } = JSON.parse(msg.data);
                    // deno-lint-ignore no-explicit-any
                    const costModels = {};
                    Object.keys(result.costModels).forEach((v) => {
                        const version = v.split(":")[1].toUpperCase();
                        const plutusVersion = "Plutus" + version;
                        costModels[plutusVersion] = result.costModels[v];
                    });
                    const [memNum, memDenom] = result.prices.memory.split("/");
                    const [stepsNum, stepsDenom] = result.prices.steps.split("/");
                    res({
                        minFeeA: parseInt(result.minFeeCoefficient),
                        minFeeB: parseInt(result.minFeeConstant),
                        maxTxSize: parseInt(result.maxTxSize),
                        maxValSize: parseInt(result.maxValueSize),
                        keyDeposit: BigInt(result.stakeKeyDeposit),
                        poolDeposit: BigInt(result.poolDeposit),
                        priceMem: parseInt(memNum) / parseInt(memDenom),
                        priceStep: parseInt(stepsNum) / parseInt(stepsDenom),
                        maxTxExMem: BigInt(result.maxExecutionUnitsPerTransaction.memory),
                        maxTxExSteps: BigInt(result.maxExecutionUnitsPerTransaction.steps),
                        coinsPerUtxoByte: BigInt(result.coinsPerUtxoByte),
                        collateralPercentage: parseInt(result.collateralPercentage),
                        maxCollateralInputs: parseInt(result.maxCollateralInputs),
                        costModels,
                    });
                    client.close();
                }
                catch (e) {
                    rej(e);
                }
            }, { once: true });
        });
    }
    async getUtxos(addressOrCredential) {
        const isAddress = typeof addressOrCredential === "string";
        const queryPredicate = isAddress
            ? addressOrCredential
            : addressOrCredential.hash;
        const result = await fetch(`${this.kupoUrl}/matches/${queryPredicate}${isAddress ? "" : "/*"}?unspent`)
            .then((res) => res.json());
        return this.kupmiosUtxosToUtxos(result);
    }
    async getUtxosWithUnit(addressOrCredential, unit) {
        const isAddress = typeof addressOrCredential === "string";
        const queryPredicate = isAddress
            ? addressOrCredential
            : addressOrCredential.hash;
        const { policyId, assetName } = (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromUnit)(unit);
        const result = await fetch(`${this.kupoUrl}/matches/${queryPredicate}${isAddress ? "" : "/*"}?unspent&policy_id=${policyId}${assetName ? `&asset_name=${assetName}` : ""}`)
            .then((res) => res.json());
        return this.kupmiosUtxosToUtxos(result);
    }
    async getUtxoByUnit(unit) {
        const { policyId, assetName } = (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromUnit)(unit);
        const result = await fetch(`${this.kupoUrl}/matches/${policyId}.${assetName ? `${assetName}` : "*"}?unspent`)
            .then((res) => res.json());
        const utxos = await this.kupmiosUtxosToUtxos(result);
        if (utxos.length > 1) {
            throw new Error("Unit needs to be an NFT or only held by one address.");
        }
        return utxos[0];
    }
    async getUtxosByOutRef(outRefs) {
        const queryHashes = [...new Set(outRefs.map((outRef) => outRef.txHash))];
        const utxos = await Promise.all(queryHashes.map(async (txHash) => {
            const result = await fetch(`${this.kupoUrl}/matches/*@${txHash}?unspent`).then((res) => res.json());
            return this.kupmiosUtxosToUtxos(result);
        }));
        return utxos.reduce((acc, utxos) => acc.concat(utxos), []).filter((utxo) => outRefs.some((outRef) => utxo.txHash === outRef.txHash && utxo.outputIndex === outRef.outputIndex));
    }
    async getDelegation(rewardAddress) {
        const client = await this.ogmiosWsp("Query", {
            query: { "delegationsAndRewards": [rewardAddress] },
        });
        return new Promise((res, rej) => {
            client.addEventListener("message", (msg) => {
                try {
                    const { result } = JSON.parse(msg.data);
                    const delegation = (result ? Object.values(result)[0] : {});
                    res({
                        poolId: delegation?.delegate || null,
                        rewards: BigInt(delegation?.rewards || 0),
                    });
                    client.close();
                }
                catch (e) {
                    rej(e);
                }
            }, { once: true });
        });
    }
    async getDatum(datumHash) {
        const result = await fetch(`${this.kupoUrl}/datums/${datumHash}`).then((res) => res.json());
        if (!result || !result.datum) {
            throw new Error(`No datum found for datum hash: ${datumHash}`);
        }
        return result.datum;
    }
    awaitTx(txHash, checkInterval = 3000) {
        return new Promise((res) => {
            const confirmation = setInterval(async () => {
                const isConfirmed = await fetch(`${this.kupoUrl}/matches/*@${txHash}?unspent`).then((res) => res.json());
                if (isConfirmed && isConfirmed.length > 0) {
                    clearInterval(confirmation);
                    res(true);
                    return;
                }
            }, checkInterval);
        });
    }
    async submitTx(tx) {
        const client = await this.ogmiosWsp("SubmitTx", {
            submit: tx,
        });
        return new Promise((res, rej) => {
            client.addEventListener("message", (msg) => {
                try {
                    const { result } = JSON.parse(msg.data);
                    if (result.SubmitSuccess)
                        res(result.SubmitSuccess.txId);
                    else
                        rej(result.SubmitFail);
                    client.close();
                }
                catch (e) {
                    rej(e);
                }
            }, { once: true });
        });
    }
    kupmiosUtxosToUtxos(utxos) {
        // deno-lint-ignore no-explicit-any
        return Promise.all(utxos.map(async (utxo) => {
            return ({
                txHash: utxo.transaction_id,
                outputIndex: parseInt(utxo.output_index),
                address: utxo.address,
                assets: (() => {
                    const a = { lovelace: BigInt(utxo.value.coins) };
                    Object.keys(utxo.value.assets).forEach((unit) => {
                        a[unit.replace(".", "")] = BigInt(utxo.value.assets[unit]);
                    });
                    return a;
                })(),
                datumHash: utxo?.datum_type === "hash" ? utxo.datum_hash : null,
                datum: utxo?.datum_type === "inline"
                    ? await this.getDatum(utxo.datum_hash)
                    : null,
                scriptRef: utxo.script_hash &&
                    (await (async () => {
                        const { script, language, } = await fetch(`${this.kupoUrl}/scripts/${utxo.script_hash}`).then((res) => res.json());
                        if (language === "native") {
                            return { type: "Native", script };
                        }
                        else if (language === "plutus:v1") {
                            return {
                                type: "PlutusV1",
                                script: (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.toHex)(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusScript["new"]((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(script)).to_bytes()),
                            };
                        }
                        else if (language === "plutus:v2") {
                            return {
                                type: "PlutusV2",
                                script: (0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.toHex)(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.PlutusScript["new"]((0,_utils_mod_js__WEBPACK_IMPORTED_MODULE_1__.fromHex)(script)).to_bytes()),
                            };
                        }
                    })()),
            });
        }));
    }
    async ogmiosWsp(methodname, args) {
        const client = new WebSocket(this.ogmiosUrl);
        await new Promise((res) => {
            client.addEventListener("open", () => res(1), { once: true });
        });
        client.send(JSON.stringify({
            type: "jsonwsp/request",
            version: "1.0",
            servicename: "ogmios",
            methodname,
            args,
        }));
        return client;
    }
}

__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/provider/mod.js":
/*!************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/provider/mod.js ***!
  \************************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "Blockfrost": () => (/* reexport safe */ _blockfrost_js__WEBPACK_IMPORTED_MODULE_0__.Blockfrost),
/* harmony export */   "Emulator": () => (/* reexport safe */ _emulator_js__WEBPACK_IMPORTED_MODULE_2__.Emulator),
/* harmony export */   "Kupmios": () => (/* reexport safe */ _kupmios_js__WEBPACK_IMPORTED_MODULE_1__.Kupmios),
/* harmony export */   "datumJsonToCbor": () => (/* reexport safe */ _blockfrost_js__WEBPACK_IMPORTED_MODULE_0__.datumJsonToCbor)
/* harmony export */ });
/* harmony import */ var _blockfrost_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./blockfrost.js */ "./node_modules/lucid-cardano/esm/src/provider/blockfrost.js");
/* harmony import */ var _kupmios_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./kupmios.js */ "./node_modules/lucid-cardano/esm/src/provider/kupmios.js");
/* harmony import */ var _emulator_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./emulator.js */ "./node_modules/lucid-cardano/esm/src/provider/emulator.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_blockfrost_js__WEBPACK_IMPORTED_MODULE_0__, _kupmios_js__WEBPACK_IMPORTED_MODULE_1__, _emulator_js__WEBPACK_IMPORTED_MODULE_2__]);
([_blockfrost_js__WEBPACK_IMPORTED_MODULE_0__, _kupmios_js__WEBPACK_IMPORTED_MODULE_1__, _emulator_js__WEBPACK_IMPORTED_MODULE_2__] = __webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__);




__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/types/global.js":
/*!************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/types/global.js ***!
  \************************************************************/
/***/ ((__unused_webpack___webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);



/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/types/mod.js":
/*!*********************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/types/mod.js ***!
  \*********************************************************/
/***/ ((__unused_webpack___webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);
/* harmony import */ var _types_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./types.js */ "./node_modules/lucid-cardano/esm/src/types/types.js");
/* harmony import */ var _global_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./global.js */ "./node_modules/lucid-cardano/esm/src/types/global.js");




/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/types/types.js":
/*!***********************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/types/types.js ***!
  \***********************************************************/
/***/ ((__unused_webpack___webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.r(__webpack_exports__);



/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/utils/cost_model.js":
/*!****************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/utils/cost_model.js ***!
  \****************************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "PROTOCOL_PARAMETERS_DEFAULT": () => (/* binding */ PROTOCOL_PARAMETERS_DEFAULT),
/* harmony export */   "createCostModels": () => (/* binding */ createCostModels)
/* harmony export */ });
/* harmony import */ var _core_mod_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../core/mod.js */ "./node_modules/lucid-cardano/esm/src/core/mod.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_core_mod_js__WEBPACK_IMPORTED_MODULE_0__]);
_core_mod_js__WEBPACK_IMPORTED_MODULE_0__ = (__webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__)[0];

function createCostModels(costModels) {
    const costmdls = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Costmdls["new"]();
    // add plutus v1
    const costmdlV1 = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.CostModel["new"]();
    Object.values(costModels.PlutusV1).forEach((cost, index) => {
        costmdlV1.set(index, _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Int["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(cost.toString())));
    });
    costmdls.insert(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Language.new_plutus_v1(), costmdlV1);
    // add plutus v2
    const costmdlV2 = _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.CostModel.new_plutus_v2();
    Object.values(costModels.PlutusV2 || []).forEach((cost, index) => {
        costmdlV2.set(index, _core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Int["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.BigNum.from_str(cost.toString())));
    });
    costmdls.insert(_core_mod_js__WEBPACK_IMPORTED_MODULE_0__.C.Language.new_plutus_v2(), costmdlV2);
    return costmdls;
}
const PROTOCOL_PARAMETERS_DEFAULT = {
    minFeeA: 44,
    minFeeB: 155381,
    maxTxSize: 16384,
    maxValSize: 5000,
    keyDeposit: 2000000n,
    poolDeposit: 500000000n,
    priceMem: 0.0577,
    priceStep: 0.0000721,
    maxTxExMem: 14000000n,
    maxTxExSteps: 10000000000n,
    coinsPerUtxoByte: 4310n,
    collateralPercentage: 150,
    maxCollateralInputs: 3,
    costModels: {
        PlutusV1: {
            "addInteger-cpu-arguments-intercept": 205665,
            "addInteger-cpu-arguments-slope": 812,
            "addInteger-memory-arguments-intercept": 1,
            "addInteger-memory-arguments-slope": 1,
            "appendByteString-cpu-arguments-intercept": 1000,
            "appendByteString-cpu-arguments-slope": 571,
            "appendByteString-memory-arguments-intercept": 0,
            "appendByteString-memory-arguments-slope": 1,
            "appendString-cpu-arguments-intercept": 1000,
            "appendString-cpu-arguments-slope": 24177,
            "appendString-memory-arguments-intercept": 4,
            "appendString-memory-arguments-slope": 1,
            "bData-cpu-arguments": 1000,
            "bData-memory-arguments": 32,
            "blake2b_256-cpu-arguments-intercept": 117366,
            "blake2b_256-cpu-arguments-slope": 10475,
            "blake2b_256-memory-arguments": 4,
            "cekApplyCost-exBudgetCPU": 23000,
            "cekApplyCost-exBudgetMemory": 100,
            "cekBuiltinCost-exBudgetCPU": 23000,
            "cekBuiltinCost-exBudgetMemory": 100,
            "cekConstCost-exBudgetCPU": 23000,
            "cekConstCost-exBudgetMemory": 100,
            "cekDelayCost-exBudgetCPU": 23000,
            "cekDelayCost-exBudgetMemory": 100,
            "cekForceCost-exBudgetCPU": 23000,
            "cekForceCost-exBudgetMemory": 100,
            "cekLamCost-exBudgetCPU": 23000,
            "cekLamCost-exBudgetMemory": 100,
            "cekStartupCost-exBudgetCPU": 100,
            "cekStartupCost-exBudgetMemory": 100,
            "cekVarCost-exBudgetCPU": 23000,
            "cekVarCost-exBudgetMemory": 100,
            "chooseData-cpu-arguments": 19537,
            "chooseData-memory-arguments": 32,
            "chooseList-cpu-arguments": 175354,
            "chooseList-memory-arguments": 32,
            "chooseUnit-cpu-arguments": 46417,
            "chooseUnit-memory-arguments": 4,
            "consByteString-cpu-arguments-intercept": 221973,
            "consByteString-cpu-arguments-slope": 511,
            "consByteString-memory-arguments-intercept": 0,
            "consByteString-memory-arguments-slope": 1,
            "constrData-cpu-arguments": 89141,
            "constrData-memory-arguments": 32,
            "decodeUtf8-cpu-arguments-intercept": 497525,
            "decodeUtf8-cpu-arguments-slope": 14068,
            "decodeUtf8-memory-arguments-intercept": 4,
            "decodeUtf8-memory-arguments-slope": 2,
            "divideInteger-cpu-arguments-constant": 196500,
            "divideInteger-cpu-arguments-model-arguments-intercept": 453240,
            "divideInteger-cpu-arguments-model-arguments-slope": 220,
            "divideInteger-memory-arguments-intercept": 0,
            "divideInteger-memory-arguments-minimum": 1,
            "divideInteger-memory-arguments-slope": 1,
            "encodeUtf8-cpu-arguments-intercept": 1000,
            "encodeUtf8-cpu-arguments-slope": 28662,
            "encodeUtf8-memory-arguments-intercept": 4,
            "encodeUtf8-memory-arguments-slope": 2,
            "equalsByteString-cpu-arguments-constant": 245000,
            "equalsByteString-cpu-arguments-intercept": 216773,
            "equalsByteString-cpu-arguments-slope": 62,
            "equalsByteString-memory-arguments": 1,
            "equalsData-cpu-arguments-intercept": 1060367,
            "equalsData-cpu-arguments-slope": 12586,
            "equalsData-memory-arguments": 1,
            "equalsInteger-cpu-arguments-intercept": 208512,
            "equalsInteger-cpu-arguments-slope": 421,
            "equalsInteger-memory-arguments": 1,
            "equalsString-cpu-arguments-constant": 187000,
            "equalsString-cpu-arguments-intercept": 1000,
            "equalsString-cpu-arguments-slope": 52998,
            "equalsString-memory-arguments": 1,
            "fstPair-cpu-arguments": 80436,
            "fstPair-memory-arguments": 32,
            "headList-cpu-arguments": 43249,
            "headList-memory-arguments": 32,
            "iData-cpu-arguments": 1000,
            "iData-memory-arguments": 32,
            "ifThenElse-cpu-arguments": 80556,
            "ifThenElse-memory-arguments": 1,
            "indexByteString-cpu-arguments": 57667,
            "indexByteString-memory-arguments": 4,
            "lengthOfByteString-cpu-arguments": 1000,
            "lengthOfByteString-memory-arguments": 10,
            "lessThanByteString-cpu-arguments-intercept": 197145,
            "lessThanByteString-cpu-arguments-slope": 156,
            "lessThanByteString-memory-arguments": 1,
            "lessThanEqualsByteString-cpu-arguments-intercept": 197145,
            "lessThanEqualsByteString-cpu-arguments-slope": 156,
            "lessThanEqualsByteString-memory-arguments": 1,
            "lessThanEqualsInteger-cpu-arguments-intercept": 204924,
            "lessThanEqualsInteger-cpu-arguments-slope": 473,
            "lessThanEqualsInteger-memory-arguments": 1,
            "lessThanInteger-cpu-arguments-intercept": 208896,
            "lessThanInteger-cpu-arguments-slope": 511,
            "lessThanInteger-memory-arguments": 1,
            "listData-cpu-arguments": 52467,
            "listData-memory-arguments": 32,
            "mapData-cpu-arguments": 64832,
            "mapData-memory-arguments": 32,
            "mkCons-cpu-arguments": 65493,
            "mkCons-memory-arguments": 32,
            "mkNilData-cpu-arguments": 22558,
            "mkNilData-memory-arguments": 32,
            "mkNilPairData-cpu-arguments": 16563,
            "mkNilPairData-memory-arguments": 32,
            "mkPairData-cpu-arguments": 76511,
            "mkPairData-memory-arguments": 32,
            "modInteger-cpu-arguments-constant": 196500,
            "modInteger-cpu-arguments-model-arguments-intercept": 453240,
            "modInteger-cpu-arguments-model-arguments-slope": 220,
            "modInteger-memory-arguments-intercept": 0,
            "modInteger-memory-arguments-minimum": 1,
            "modInteger-memory-arguments-slope": 1,
            "multiplyInteger-cpu-arguments-intercept": 69522,
            "multiplyInteger-cpu-arguments-slope": 11687,
            "multiplyInteger-memory-arguments-intercept": 0,
            "multiplyInteger-memory-arguments-slope": 1,
            "nullList-cpu-arguments": 60091,
            "nullList-memory-arguments": 32,
            "quotientInteger-cpu-arguments-constant": 196500,
            "quotientInteger-cpu-arguments-model-arguments-intercept": 453240,
            "quotientInteger-cpu-arguments-model-arguments-slope": 220,
            "quotientInteger-memory-arguments-intercept": 0,
            "quotientInteger-memory-arguments-minimum": 1,
            "quotientInteger-memory-arguments-slope": 1,
            "remainderInteger-cpu-arguments-constant": 196500,
            "remainderInteger-cpu-arguments-model-arguments-intercept": 453240,
            "remainderInteger-cpu-arguments-model-arguments-slope": 220,
            "remainderInteger-memory-arguments-intercept": 0,
            "remainderInteger-memory-arguments-minimum": 1,
            "remainderInteger-memory-arguments-slope": 1,
            "sha2_256-cpu-arguments-intercept": 806990,
            "sha2_256-cpu-arguments-slope": 30482,
            "sha2_256-memory-arguments": 4,
            "sha3_256-cpu-arguments-intercept": 1927926,
            "sha3_256-cpu-arguments-slope": 82523,
            "sha3_256-memory-arguments": 4,
            "sliceByteString-cpu-arguments-intercept": 265318,
            "sliceByteString-cpu-arguments-slope": 0,
            "sliceByteString-memory-arguments-intercept": 4,
            "sliceByteString-memory-arguments-slope": 0,
            "sndPair-cpu-arguments": 85931,
            "sndPair-memory-arguments": 32,
            "subtractInteger-cpu-arguments-intercept": 205665,
            "subtractInteger-cpu-arguments-slope": 812,
            "subtractInteger-memory-arguments-intercept": 1,
            "subtractInteger-memory-arguments-slope": 1,
            "tailList-cpu-arguments": 41182,
            "tailList-memory-arguments": 32,
            "trace-cpu-arguments": 212342,
            "trace-memory-arguments": 32,
            "unBData-cpu-arguments": 31220,
            "unBData-memory-arguments": 32,
            "unConstrData-cpu-arguments": 32696,
            "unConstrData-memory-arguments": 32,
            "unIData-cpu-arguments": 43357,
            "unIData-memory-arguments": 32,
            "unListData-cpu-arguments": 32247,
            "unListData-memory-arguments": 32,
            "unMapData-cpu-arguments": 38314,
            "unMapData-memory-arguments": 32,
            "verifyEd25519Signature-cpu-arguments-intercept": 9462713,
            "verifyEd25519Signature-cpu-arguments-slope": 1021,
            "verifyEd25519Signature-memory-arguments": 10,
        },
        PlutusV2: {
            "addInteger-cpu-arguments-intercept": 205665,
            "addInteger-cpu-arguments-slope": 812,
            "addInteger-memory-arguments-intercept": 1,
            "addInteger-memory-arguments-slope": 1,
            "appendByteString-cpu-arguments-intercept": 1000,
            "appendByteString-cpu-arguments-slope": 571,
            "appendByteString-memory-arguments-intercept": 0,
            "appendByteString-memory-arguments-slope": 1,
            "appendString-cpu-arguments-intercept": 1000,
            "appendString-cpu-arguments-slope": 24177,
            "appendString-memory-arguments-intercept": 4,
            "appendString-memory-arguments-slope": 1,
            "bData-cpu-arguments": 1000,
            "bData-memory-arguments": 32,
            "blake2b_256-cpu-arguments-intercept": 117366,
            "blake2b_256-cpu-arguments-slope": 10475,
            "blake2b_256-memory-arguments": 4,
            "cekApplyCost-exBudgetCPU": 23000,
            "cekApplyCost-exBudgetMemory": 100,
            "cekBuiltinCost-exBudgetCPU": 23000,
            "cekBuiltinCost-exBudgetMemory": 100,
            "cekConstCost-exBudgetCPU": 23000,
            "cekConstCost-exBudgetMemory": 100,
            "cekDelayCost-exBudgetCPU": 23000,
            "cekDelayCost-exBudgetMemory": 100,
            "cekForceCost-exBudgetCPU": 23000,
            "cekForceCost-exBudgetMemory": 100,
            "cekLamCost-exBudgetCPU": 23000,
            "cekLamCost-exBudgetMemory": 100,
            "cekStartupCost-exBudgetCPU": 100,
            "cekStartupCost-exBudgetMemory": 100,
            "cekVarCost-exBudgetCPU": 23000,
            "cekVarCost-exBudgetMemory": 100,
            "chooseData-cpu-arguments": 19537,
            "chooseData-memory-arguments": 32,
            "chooseList-cpu-arguments": 175354,
            "chooseList-memory-arguments": 32,
            "chooseUnit-cpu-arguments": 46417,
            "chooseUnit-memory-arguments": 4,
            "consByteString-cpu-arguments-intercept": 221973,
            "consByteString-cpu-arguments-slope": 511,
            "consByteString-memory-arguments-intercept": 0,
            "consByteString-memory-arguments-slope": 1,
            "constrData-cpu-arguments": 89141,
            "constrData-memory-arguments": 32,
            "decodeUtf8-cpu-arguments-intercept": 497525,
            "decodeUtf8-cpu-arguments-slope": 14068,
            "decodeUtf8-memory-arguments-intercept": 4,
            "decodeUtf8-memory-arguments-slope": 2,
            "divideInteger-cpu-arguments-constant": 196500,
            "divideInteger-cpu-arguments-model-arguments-intercept": 453240,
            "divideInteger-cpu-arguments-model-arguments-slope": 220,
            "divideInteger-memory-arguments-intercept": 0,
            "divideInteger-memory-arguments-minimum": 1,
            "divideInteger-memory-arguments-slope": 1,
            "encodeUtf8-cpu-arguments-intercept": 1000,
            "encodeUtf8-cpu-arguments-slope": 28662,
            "encodeUtf8-memory-arguments-intercept": 4,
            "encodeUtf8-memory-arguments-slope": 2,
            "equalsByteString-cpu-arguments-constant": 245000,
            "equalsByteString-cpu-arguments-intercept": 216773,
            "equalsByteString-cpu-arguments-slope": 62,
            "equalsByteString-memory-arguments": 1,
            "equalsData-cpu-arguments-intercept": 1060367,
            "equalsData-cpu-arguments-slope": 12586,
            "equalsData-memory-arguments": 1,
            "equalsInteger-cpu-arguments-intercept": 208512,
            "equalsInteger-cpu-arguments-slope": 421,
            "equalsInteger-memory-arguments": 1,
            "equalsString-cpu-arguments-constant": 187000,
            "equalsString-cpu-arguments-intercept": 1000,
            "equalsString-cpu-arguments-slope": 52998,
            "equalsString-memory-arguments": 1,
            "fstPair-cpu-arguments": 80436,
            "fstPair-memory-arguments": 32,
            "headList-cpu-arguments": 43249,
            "headList-memory-arguments": 32,
            "iData-cpu-arguments": 1000,
            "iData-memory-arguments": 32,
            "ifThenElse-cpu-arguments": 80556,
            "ifThenElse-memory-arguments": 1,
            "indexByteString-cpu-arguments": 57667,
            "indexByteString-memory-arguments": 4,
            "lengthOfByteString-cpu-arguments": 1000,
            "lengthOfByteString-memory-arguments": 10,
            "lessThanByteString-cpu-arguments-intercept": 197145,
            "lessThanByteString-cpu-arguments-slope": 156,
            "lessThanByteString-memory-arguments": 1,
            "lessThanEqualsByteString-cpu-arguments-intercept": 197145,
            "lessThanEqualsByteString-cpu-arguments-slope": 156,
            "lessThanEqualsByteString-memory-arguments": 1,
            "lessThanEqualsInteger-cpu-arguments-intercept": 204924,
            "lessThanEqualsInteger-cpu-arguments-slope": 473,
            "lessThanEqualsInteger-memory-arguments": 1,
            "lessThanInteger-cpu-arguments-intercept": 208896,
            "lessThanInteger-cpu-arguments-slope": 511,
            "lessThanInteger-memory-arguments": 1,
            "listData-cpu-arguments": 52467,
            "listData-memory-arguments": 32,
            "mapData-cpu-arguments": 64832,
            "mapData-memory-arguments": 32,
            "mkCons-cpu-arguments": 65493,
            "mkCons-memory-arguments": 32,
            "mkNilData-cpu-arguments": 22558,
            "mkNilData-memory-arguments": 32,
            "mkNilPairData-cpu-arguments": 16563,
            "mkNilPairData-memory-arguments": 32,
            "mkPairData-cpu-arguments": 76511,
            "mkPairData-memory-arguments": 32,
            "modInteger-cpu-arguments-constant": 196500,
            "modInteger-cpu-arguments-model-arguments-intercept": 453240,
            "modInteger-cpu-arguments-model-arguments-slope": 220,
            "modInteger-memory-arguments-intercept": 0,
            "modInteger-memory-arguments-minimum": 1,
            "modInteger-memory-arguments-slope": 1,
            "multiplyInteger-cpu-arguments-intercept": 69522,
            "multiplyInteger-cpu-arguments-slope": 11687,
            "multiplyInteger-memory-arguments-intercept": 0,
            "multiplyInteger-memory-arguments-slope": 1,
            "nullList-cpu-arguments": 60091,
            "nullList-memory-arguments": 32,
            "quotientInteger-cpu-arguments-constant": 196500,
            "quotientInteger-cpu-arguments-model-arguments-intercept": 453240,
            "quotientInteger-cpu-arguments-model-arguments-slope": 220,
            "quotientInteger-memory-arguments-intercept": 0,
            "quotientInteger-memory-arguments-minimum": 1,
            "quotientInteger-memory-arguments-slope": 1,
            "remainderInteger-cpu-arguments-constant": 196500,
            "remainderInteger-cpu-arguments-model-arguments-intercept": 453240,
            "remainderInteger-cpu-arguments-model-arguments-slope": 220,
            "remainderInteger-memory-arguments-intercept": 0,
            "remainderInteger-memory-arguments-minimum": 1,
            "remainderInteger-memory-arguments-slope": 1,
            "serialiseData-cpu-arguments-intercept": 1159724,
            "serialiseData-cpu-arguments-slope": 392670,
            "serialiseData-memory-arguments-intercept": 0,
            "serialiseData-memory-arguments-slope": 2,
            "sha2_256-cpu-arguments-intercept": 806990,
            "sha2_256-cpu-arguments-slope": 30482,
            "sha2_256-memory-arguments": 4,
            "sha3_256-cpu-arguments-intercept": 1927926,
            "sha3_256-cpu-arguments-slope": 82523,
            "sha3_256-memory-arguments": 4,
            "sliceByteString-cpu-arguments-intercept": 265318,
            "sliceByteString-cpu-arguments-slope": 0,
            "sliceByteString-memory-arguments-intercept": 4,
            "sliceByteString-memory-arguments-slope": 0,
            "sndPair-cpu-arguments": 85931,
            "sndPair-memory-arguments": 32,
            "subtractInteger-cpu-arguments-intercept": 205665,
            "subtractInteger-cpu-arguments-slope": 812,
            "subtractInteger-memory-arguments-intercept": 1,
            "subtractInteger-memory-arguments-slope": 1,
            "tailList-cpu-arguments": 41182,
            "tailList-memory-arguments": 32,
            "trace-cpu-arguments": 212342,
            "trace-memory-arguments": 32,
            "unBData-cpu-arguments": 31220,
            "unBData-memory-arguments": 32,
            "unConstrData-cpu-arguments": 32696,
            "unConstrData-memory-arguments": 32,
            "unIData-cpu-arguments": 43357,
            "unIData-memory-arguments": 32,
            "unListData-cpu-arguments": 32247,
            "unListData-memory-arguments": 32,
            "unMapData-cpu-arguments": 38314,
            "unMapData-memory-arguments": 32,
            "verifyEcdsaSecp256k1Signature-cpu-arguments": 35892428,
            "verifyEcdsaSecp256k1Signature-memory-arguments": 10,
            "verifyEd25519Signature-cpu-arguments-intercept": 57996947,
            "verifyEd25519Signature-cpu-arguments-slope": 18975,
            "verifyEd25519Signature-memory-arguments": 10,
            "verifySchnorrSecp256k1Signature-cpu-arguments-intercept": 38887044,
            "verifySchnorrSecp256k1Signature-cpu-arguments-slope": 32947,
            "verifySchnorrSecp256k1Signature-memory-arguments": 10,
        },
    },
};

__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/utils/merkle_tree.js":
/*!*****************************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/utils/merkle_tree.js ***!
  \*****************************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "MerkleTree": () => (/* binding */ MerkleTree),
/* harmony export */   "combineHash": () => (/* binding */ combineHash),
/* harmony export */   "concat": () => (/* reexport safe */ _deps_deno_land_std_0_148_0_bytes_mod_js__WEBPACK_IMPORTED_MODULE_0__.concat),
/* harmony export */   "equals": () => (/* reexport safe */ _deps_deno_land_std_0_148_0_bytes_mod_js__WEBPACK_IMPORTED_MODULE_0__.equals),
/* harmony export */   "sha256": () => (/* binding */ sha256)
/* harmony export */ });
/* harmony import */ var _deps_deno_land_std_0_148_0_bytes_mod_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../../deps/deno.land/std@0.148.0/bytes/mod.js */ "./node_modules/lucid-cardano/esm/deps/deno.land/std@0.148.0/bytes/mod.js");
/* harmony import */ var _deps_deno_land_std_0_153_0_hash_sha256_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../../deps/deno.land/std@0.153.0/hash/sha256.js */ "./node_modules/lucid-cardano/esm/deps/deno.land/std@0.153.0/hash/sha256.js");
/* harmony import */ var _utils_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./utils.js */ "./node_modules/lucid-cardano/esm/src/utils/utils.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_utils_js__WEBPACK_IMPORTED_MODULE_2__]);
_utils_js__WEBPACK_IMPORTED_MODULE_2__ = (__webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__)[0];
// Haskell implementation: https://github.com/input-output-hk/hydra-poc/blob/master/plutus-merkle-tree/src/Plutus/MerkleTree.hs



class MerkleTree {
    /** Construct Merkle tree from data, which get hashed with sha256 */
    constructor(data) {
        Object.defineProperty(this, "root", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        this.root = MerkleTree.buildRecursively(data.map((d) => sha256(d)));
    }
    /** Construct Merkle tree from sha256 hashes */
    static fromHashes(hashes) {
        return new this(hashes);
    }
    static buildRecursively(hashes) {
        if (hashes.length <= 0)
            return null;
        if (hashes.length === 1) {
            return {
                node: hashes[0],
                left: null,
                right: null,
            };
        }
        const cutoff = Math.floor(hashes.length / 2);
        const [left, right] = [hashes.slice(0, cutoff), hashes.slice(cutoff)];
        const lnode = this.buildRecursively(left);
        const rnode = this.buildRecursively(right);
        if (lnode === null || rnode === null)
            return null;
        return {
            node: combineHash(lnode.node, rnode.node),
            left: lnode,
            right: rnode,
        };
    }
    rootHash() {
        if (this.root === null)
            throw new Error("Merkle tree root hash not found.");
        return this.root.node;
    }
    getProof(data) {
        const hash = sha256(data);
        const proof = [];
        const searchRecursively = (tree) => {
            if (tree && (0,_deps_deno_land_std_0_148_0_bytes_mod_js__WEBPACK_IMPORTED_MODULE_0__.equals)(tree.node, hash))
                return true;
            if (tree?.right) {
                if (searchRecursively(tree.left)) {
                    proof.push({ right: tree.right.node });
                    return true;
                }
            }
            if (tree?.left) {
                if (searchRecursively(tree.right)) {
                    proof.push({ left: tree.left.node });
                    return true;
                }
            }
        };
        searchRecursively(this.root);
        return proof;
    }
    size() {
        const searchRecursively = (tree) => {
            if (tree === null)
                return 0;
            return 1 + searchRecursively(tree.left) + searchRecursively(tree.right);
        };
        return searchRecursively(this.root);
    }
    static verify(data, rootHash, proof) {
        const hash = sha256(data);
        const searchRecursively = (rootHash2, proof) => {
            if (proof.length <= 0)
                return (0,_deps_deno_land_std_0_148_0_bytes_mod_js__WEBPACK_IMPORTED_MODULE_0__.equals)(rootHash, rootHash2);
            const [h, t] = [proof[0], proof.slice(1)];
            if (h.left) {
                return searchRecursively(combineHash(h.left, rootHash2), t);
            }
            if (h.right) {
                return searchRecursively(combineHash(rootHash2, h.right), t);
            }
            return false;
        };
        return searchRecursively(hash, proof);
    }
    toString() {
        // deno-lint-ignore no-explicit-any
        const searchRecursively = (tree) => {
            if (tree === null)
                return null;
            return {
                node: (0,_utils_js__WEBPACK_IMPORTED_MODULE_2__.toHex)(tree.node),
                left: searchRecursively(tree.left),
                right: searchRecursively(tree.right),
            };
        };
        return JSON.stringify(searchRecursively(this.root), null, 2);
    }
}

function sha256(data) {
    return new Uint8Array(new _deps_deno_land_std_0_153_0_hash_sha256_js__WEBPACK_IMPORTED_MODULE_1__.Sha256().update(data).arrayBuffer());
}
function combineHash(hash1, hash2) {
    return sha256((0,_deps_deno_land_std_0_148_0_bytes_mod_js__WEBPACK_IMPORTED_MODULE_0__.concat)(hash1, hash2));
}

__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/utils/mod.js":
/*!*********************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/utils/mod.js ***!
  \*********************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "MerkleTree": () => (/* reexport safe */ _merkle_tree_js__WEBPACK_IMPORTED_MODULE_2__.MerkleTree),
/* harmony export */   "PROTOCOL_PARAMETERS_DEFAULT": () => (/* reexport safe */ _cost_model_js__WEBPACK_IMPORTED_MODULE_0__.PROTOCOL_PARAMETERS_DEFAULT),
/* harmony export */   "Utils": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.Utils),
/* harmony export */   "applyDoubleCborEncoding": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.applyDoubleCborEncoding),
/* harmony export */   "applyParamsToScript": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.applyParamsToScript),
/* harmony export */   "assetsToValue": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.assetsToValue),
/* harmony export */   "combineHash": () => (/* reexport safe */ _merkle_tree_js__WEBPACK_IMPORTED_MODULE_2__.combineHash),
/* harmony export */   "concat": () => (/* reexport safe */ _merkle_tree_js__WEBPACK_IMPORTED_MODULE_2__.concat),
/* harmony export */   "coreToUtxo": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.coreToUtxo),
/* harmony export */   "createCostModels": () => (/* reexport safe */ _cost_model_js__WEBPACK_IMPORTED_MODULE_0__.createCostModels),
/* harmony export */   "equals": () => (/* reexport safe */ _merkle_tree_js__WEBPACK_IMPORTED_MODULE_2__.equals),
/* harmony export */   "fromHex": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.fromHex),
/* harmony export */   "fromLabel": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.fromLabel),
/* harmony export */   "fromScriptRef": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.fromScriptRef),
/* harmony export */   "fromText": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.fromText),
/* harmony export */   "fromUnit": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.fromUnit),
/* harmony export */   "generatePrivateKey": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.generatePrivateKey),
/* harmony export */   "generateSeedPhrase": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.generateSeedPhrase),
/* harmony export */   "getAddressDetails": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.getAddressDetails),
/* harmony export */   "nativeScriptFromJson": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.nativeScriptFromJson),
/* harmony export */   "networkToId": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.networkToId),
/* harmony export */   "paymentCredentialOf": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.paymentCredentialOf),
/* harmony export */   "sha256": () => (/* reexport safe */ _merkle_tree_js__WEBPACK_IMPORTED_MODULE_2__.sha256),
/* harmony export */   "stakeCredentialOf": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.stakeCredentialOf),
/* harmony export */   "toHex": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.toHex),
/* harmony export */   "toLabel": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.toLabel),
/* harmony export */   "toPublicKey": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.toPublicKey),
/* harmony export */   "toScriptRef": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.toScriptRef),
/* harmony export */   "toText": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.toText),
/* harmony export */   "toUnit": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.toUnit),
/* harmony export */   "utxoToCore": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.utxoToCore),
/* harmony export */   "valueToAssets": () => (/* reexport safe */ _utils_js__WEBPACK_IMPORTED_MODULE_1__.valueToAssets)
/* harmony export */ });
/* harmony import */ var _cost_model_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ./cost_model.js */ "./node_modules/lucid-cardano/esm/src/utils/cost_model.js");
/* harmony import */ var _utils_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ./utils.js */ "./node_modules/lucid-cardano/esm/src/utils/utils.js");
/* harmony import */ var _merkle_tree_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ./merkle_tree.js */ "./node_modules/lucid-cardano/esm/src/utils/merkle_tree.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_cost_model_js__WEBPACK_IMPORTED_MODULE_0__, _utils_js__WEBPACK_IMPORTED_MODULE_1__, _merkle_tree_js__WEBPACK_IMPORTED_MODULE_2__]);
([_cost_model_js__WEBPACK_IMPORTED_MODULE_0__, _utils_js__WEBPACK_IMPORTED_MODULE_1__, _merkle_tree_js__WEBPACK_IMPORTED_MODULE_2__] = __webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__);




__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ }),

/***/ "./node_modules/lucid-cardano/esm/src/utils/utils.js":
/*!***********************************************************!*\
  !*** ./node_modules/lucid-cardano/esm/src/utils/utils.js ***!
  \***********************************************************/
/***/ ((__webpack_module__, __webpack_exports__, __webpack_require__) => {

__webpack_require__.a(__webpack_module__, async (__webpack_handle_async_dependencies__, __webpack_async_result__) => { try {
__webpack_require__.r(__webpack_exports__);
/* harmony export */ __webpack_require__.d(__webpack_exports__, {
/* harmony export */   "Utils": () => (/* binding */ Utils),
/* harmony export */   "applyDoubleCborEncoding": () => (/* binding */ applyDoubleCborEncoding),
/* harmony export */   "applyParamsToScript": () => (/* binding */ applyParamsToScript),
/* harmony export */   "assetsToValue": () => (/* binding */ assetsToValue),
/* harmony export */   "coreToUtxo": () => (/* binding */ coreToUtxo),
/* harmony export */   "fromHex": () => (/* binding */ fromHex),
/* harmony export */   "fromLabel": () => (/* binding */ fromLabel),
/* harmony export */   "fromScriptRef": () => (/* binding */ fromScriptRef),
/* harmony export */   "fromText": () => (/* binding */ fromText),
/* harmony export */   "fromUnit": () => (/* binding */ fromUnit),
/* harmony export */   "generatePrivateKey": () => (/* binding */ generatePrivateKey),
/* harmony export */   "generateSeedPhrase": () => (/* binding */ generateSeedPhrase),
/* harmony export */   "getAddressDetails": () => (/* binding */ getAddressDetails),
/* harmony export */   "nativeScriptFromJson": () => (/* binding */ nativeScriptFromJson),
/* harmony export */   "networkToId": () => (/* binding */ networkToId),
/* harmony export */   "paymentCredentialOf": () => (/* binding */ paymentCredentialOf),
/* harmony export */   "stakeCredentialOf": () => (/* binding */ stakeCredentialOf),
/* harmony export */   "toHex": () => (/* binding */ toHex),
/* harmony export */   "toLabel": () => (/* binding */ toLabel),
/* harmony export */   "toPublicKey": () => (/* binding */ toPublicKey),
/* harmony export */   "toScriptRef": () => (/* binding */ toScriptRef),
/* harmony export */   "toText": () => (/* binding */ toText),
/* harmony export */   "toUnit": () => (/* binding */ toUnit),
/* harmony export */   "utxoToCore": () => (/* binding */ utxoToCore),
/* harmony export */   "valueToAssets": () => (/* binding */ valueToAssets)
/* harmony export */ });
/* harmony import */ var _deps_deno_land_std_0_100_0_encoding_hex_js__WEBPACK_IMPORTED_MODULE_0__ = __webpack_require__(/*! ../../deps/deno.land/std@0.100.0/encoding/hex.js */ "./node_modules/lucid-cardano/esm/deps/deno.land/std@0.100.0/encoding/hex.js");
/* harmony import */ var _core_mod_js__WEBPACK_IMPORTED_MODULE_1__ = __webpack_require__(/*! ../core/mod.js */ "./node_modules/lucid-cardano/esm/src/core/mod.js");
/* harmony import */ var _misc_bip39_js__WEBPACK_IMPORTED_MODULE_2__ = __webpack_require__(/*! ../misc/bip39.js */ "./node_modules/lucid-cardano/esm/src/misc/bip39.js");
/* harmony import */ var _misc_crc8_js__WEBPACK_IMPORTED_MODULE_3__ = __webpack_require__(/*! ../misc/crc8.js */ "./node_modules/lucid-cardano/esm/src/misc/crc8.js");
/* harmony import */ var _plutus_time_js__WEBPACK_IMPORTED_MODULE_4__ = __webpack_require__(/*! ../plutus/time.js */ "./node_modules/lucid-cardano/esm/src/plutus/time.js");
/* harmony import */ var _plutus_data_js__WEBPACK_IMPORTED_MODULE_5__ = __webpack_require__(/*! ../plutus/data.js */ "./node_modules/lucid-cardano/esm/src/plutus/data.js");
var __webpack_async_dependencies__ = __webpack_handle_async_dependencies__([_core_mod_js__WEBPACK_IMPORTED_MODULE_1__, _misc_bip39_js__WEBPACK_IMPORTED_MODULE_2__, _plutus_data_js__WEBPACK_IMPORTED_MODULE_5__]);
([_core_mod_js__WEBPACK_IMPORTED_MODULE_1__, _misc_bip39_js__WEBPACK_IMPORTED_MODULE_2__, _plutus_data_js__WEBPACK_IMPORTED_MODULE_5__] = __webpack_async_dependencies__.then ? (await __webpack_async_dependencies__)() : __webpack_async_dependencies__);






class Utils {
    constructor(lucid) {
        Object.defineProperty(this, "lucid", {
            enumerable: true,
            configurable: true,
            writable: true,
            value: void 0
        });
        this.lucid = lucid;
    }
    validatorToAddress(validator, stakeCredential) {
        const validatorHash = this.validatorToScriptHash(validator);
        if (stakeCredential) {
            return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.BaseAddress["new"](networkToId(this.lucid.network), _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.StakeCredential.from_scripthash(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ScriptHash.from_hex(validatorHash)), stakeCredential.type === "Key"
                ? _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.StakeCredential.from_keyhash(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.Ed25519KeyHash.from_hex(stakeCredential.hash))
                : _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.StakeCredential.from_scripthash(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ScriptHash.from_hex(stakeCredential.hash)))
                .to_address()
                .to_bech32(undefined);
        }
        else {
            return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.EnterpriseAddress["new"](networkToId(this.lucid.network), _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.StakeCredential.from_scripthash(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ScriptHash.from_hex(validatorHash)))
                .to_address()
                .to_bech32(undefined);
        }
    }
    credentialToAddress(paymentCredential, stakeCredential) {
        if (stakeCredential) {
            return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.BaseAddress["new"](networkToId(this.lucid.network), paymentCredential.type === "Key"
                ? _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.StakeCredential.from_keyhash(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.Ed25519KeyHash.from_hex(paymentCredential.hash))
                : _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.StakeCredential.from_scripthash(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ScriptHash.from_hex(paymentCredential.hash)), stakeCredential.type === "Key"
                ? _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.StakeCredential.from_keyhash(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.Ed25519KeyHash.from_hex(stakeCredential.hash))
                : _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.StakeCredential.from_scripthash(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ScriptHash.from_hex(stakeCredential.hash)))
                .to_address()
                .to_bech32(undefined);
        }
        else {
            return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.EnterpriseAddress["new"](networkToId(this.lucid.network), paymentCredential.type === "Key"
                ? _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.StakeCredential.from_keyhash(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.Ed25519KeyHash.from_hex(paymentCredential.hash))
                : _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.StakeCredential.from_scripthash(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ScriptHash.from_hex(paymentCredential.hash)))
                .to_address()
                .to_bech32(undefined);
        }
    }
    validatorToRewardAddress(validator) {
        const validatorHash = this.validatorToScriptHash(validator);
        return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.RewardAddress["new"](networkToId(this.lucid.network), _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.StakeCredential.from_scripthash(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ScriptHash.from_hex(validatorHash)))
            .to_address()
            .to_bech32(undefined);
    }
    credentialToRewardAddress(stakeCredential) {
        return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.RewardAddress["new"](networkToId(this.lucid.network), stakeCredential.type === "Key"
            ? _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.StakeCredential.from_keyhash(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.Ed25519KeyHash.from_hex(stakeCredential.hash))
            : _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.StakeCredential.from_scripthash(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ScriptHash.from_hex(stakeCredential.hash)))
            .to_address()
            .to_bech32(undefined);
    }
    validatorToScriptHash(validator) {
        switch (validator.type) {
            case "Native":
                return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.NativeScript.from_bytes(fromHex(validator.script))
                    .hash(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ScriptHashNamespace.NativeScript)
                    .to_hex();
            case "PlutusV1":
                return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusScript.from_bytes(fromHex(applyDoubleCborEncoding(validator.script)))
                    .hash(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ScriptHashNamespace.PlutusV1)
                    .to_hex();
            case "PlutusV2":
                return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusScript.from_bytes(fromHex(applyDoubleCborEncoding(validator.script)))
                    .hash(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ScriptHashNamespace.PlutusV2)
                    .to_hex();
            default:
                throw new Error("No variant matched");
        }
    }
    mintingPolicyToId(mintingPolicy) {
        return this.validatorToScriptHash(mintingPolicy);
    }
    datumToHash(datum) {
        return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.hash_plutus_data(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusData.from_bytes(fromHex(datum))).to_hex();
    }
    scriptHashToCredential(scriptHash) {
        return {
            type: "Script",
            hash: scriptHash,
        };
    }
    keyHashToCredential(keyHash) {
        return {
            type: "Key",
            hash: keyHash,
        };
    }
    generatePrivateKey() {
        return generatePrivateKey();
    }
    generateSeedPhrase() {
        return generateSeedPhrase();
    }
    unixTimeToSlot(unixTime) {
        return (0,_plutus_time_js__WEBPACK_IMPORTED_MODULE_4__.unixTimeToEnclosingSlot)(unixTime, _plutus_time_js__WEBPACK_IMPORTED_MODULE_4__.SLOT_CONFIG_NETWORK[this.lucid.network]);
    }
    slotToUnixTime(slot) {
        return (0,_plutus_time_js__WEBPACK_IMPORTED_MODULE_4__.slotToBeginUnixTime)(slot, _plutus_time_js__WEBPACK_IMPORTED_MODULE_4__.SLOT_CONFIG_NETWORK[this.lucid.network]);
    }
    /** Address can be in Bech32 or Hex. */
    getAddressDetails(address) {
        return getAddressDetails(address);
    }
    /**
     * Convert a native script from Json to the Hex representation.
     * It follows this Json format: https://github.com/input-output-hk/cardano-node/blob/master/doc/reference/simple-scripts.md
     */
    nativeScriptFromJson(nativeScript) {
        return nativeScriptFromJson(nativeScript);
    }
    paymentCredentialOf(address) {
        return paymentCredentialOf(address);
    }
    stakeCredentialOf(rewardAddress) {
        return stakeCredentialOf(rewardAddress);
    }
}
function addressFromHexOrBech32(address) {
    try {
        return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.Address.from_bytes(fromHex(address));
    }
    catch (_e) {
        try {
            return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.Address.from_bech32(address);
        }
        catch (_e) {
            throw new Error("Could not deserialize address.");
        }
    }
}
/** Address can be in Bech32 or Hex. */
function getAddressDetails(address) {
    // Base Address
    try {
        const parsedAddress = _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.BaseAddress.from_address(addressFromHexOrBech32(address));
        const paymentCredential = parsedAddress.payment_cred().kind() === 0
            ? {
                type: "Key",
                hash: toHex(parsedAddress.payment_cred().to_keyhash().to_bytes()),
            }
            : {
                type: "Script",
                hash: toHex(parsedAddress.payment_cred().to_scripthash().to_bytes()),
            };
        const stakeCredential = parsedAddress.stake_cred().kind() === 0
            ? {
                type: "Key",
                hash: toHex(parsedAddress.stake_cred().to_keyhash().to_bytes()),
            }
            : {
                type: "Script",
                hash: toHex(parsedAddress.stake_cred().to_scripthash().to_bytes()),
            };
        return {
            type: "Base",
            networkId: parsedAddress.to_address().network_id(),
            address: {
                bech32: parsedAddress.to_address().to_bech32(undefined),
                hex: toHex(parsedAddress.to_address().to_bytes()),
            },
            paymentCredential,
            stakeCredential,
        };
    }
    catch (_e) { /* pass */ }
    // Enterprise Address
    try {
        const parsedAddress = _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.EnterpriseAddress.from_address(addressFromHexOrBech32(address));
        const paymentCredential = parsedAddress.payment_cred().kind() === 0
            ? {
                type: "Key",
                hash: toHex(parsedAddress.payment_cred().to_keyhash().to_bytes()),
            }
            : {
                type: "Script",
                hash: toHex(parsedAddress.payment_cred().to_scripthash().to_bytes()),
            };
        return {
            type: "Enterprise",
            networkId: parsedAddress.to_address().network_id(),
            address: {
                bech32: parsedAddress.to_address().to_bech32(undefined),
                hex: toHex(parsedAddress.to_address().to_bytes()),
            },
            paymentCredential,
        };
    }
    catch (_e) { /* pass */ }
    // Pointer Address
    try {
        const parsedAddress = _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PointerAddress.from_address(addressFromHexOrBech32(address));
        const paymentCredential = parsedAddress.payment_cred().kind() === 0
            ? {
                type: "Key",
                hash: toHex(parsedAddress.payment_cred().to_keyhash().to_bytes()),
            }
            : {
                type: "Script",
                hash: toHex(parsedAddress.payment_cred().to_scripthash().to_bytes()),
            };
        return {
            type: "Pointer",
            networkId: parsedAddress.to_address().network_id(),
            address: {
                bech32: parsedAddress.to_address().to_bech32(undefined),
                hex: toHex(parsedAddress.to_address().to_bytes()),
            },
            paymentCredential,
        };
    }
    catch (_e) { /* pass */ }
    // Reward Address
    try {
        const parsedAddress = _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.RewardAddress.from_address(addressFromHexOrBech32(address));
        const stakeCredential = parsedAddress.payment_cred().kind() === 0
            ? {
                type: "Key",
                hash: toHex(parsedAddress.payment_cred().to_keyhash().to_bytes()),
            }
            : {
                type: "Script",
                hash: toHex(parsedAddress.payment_cred().to_scripthash().to_bytes()),
            };
        return {
            type: "Reward",
            networkId: parsedAddress.to_address().network_id(),
            address: {
                bech32: parsedAddress.to_address().to_bech32(undefined),
                hex: toHex(parsedAddress.to_address().to_bytes()),
            },
            stakeCredential,
        };
    }
    catch (_e) { /* pass */ }
    throw new Error("No address type matched for: " + address);
}
function paymentCredentialOf(address) {
    const { paymentCredential } = getAddressDetails(address);
    if (!paymentCredential) {
        throw new Error("The specified address does not contain a payment credential.");
    }
    return paymentCredential;
}
function stakeCredentialOf(rewardAddress) {
    const { stakeCredential } = getAddressDetails(rewardAddress);
    if (!stakeCredential) {
        throw new Error("The specified address does not contain a stake credential.");
    }
    return stakeCredential;
}
function generatePrivateKey() {
    return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PrivateKey.generate_ed25519().to_bech32();
}
function generateSeedPhrase() {
    return (0,_misc_bip39_js__WEBPACK_IMPORTED_MODULE_2__.generateMnemonic)(256);
}
function valueToAssets(value) {
    const assets = {};
    assets["lovelace"] = BigInt(value.coin().to_str());
    const ma = value.multiasset();
    if (ma) {
        const multiAssets = ma.keys();
        for (let j = 0; j < multiAssets.len(); j++) {
            const policy = multiAssets.get(j);
            const policyAssets = ma.get(policy);
            const assetNames = policyAssets.keys();
            for (let k = 0; k < assetNames.len(); k++) {
                const policyAsset = assetNames.get(k);
                const quantity = policyAssets.get(policyAsset);
                const unit = toHex(policy.to_bytes()) + toHex(policyAsset.name());
                assets[unit] = BigInt(quantity.to_str());
            }
        }
    }
    return assets;
}
function assetsToValue(assets) {
    const multiAsset = _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.MultiAsset["new"]();
    const lovelace = assets["lovelace"];
    const units = Object.keys(assets);
    const policies = Array.from(new Set(units
        .filter((unit) => unit !== "lovelace")
        .map((unit) => unit.slice(0, 56))));
    policies.forEach((policy) => {
        const policyUnits = units.filter((unit) => unit.slice(0, 56) === policy);
        const assetsValue = _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.Assets["new"]();
        policyUnits.forEach((unit) => {
            assetsValue.insert(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.AssetName["new"](fromHex(unit.slice(56))), _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.BigNum.from_str(assets[unit].toString()));
        });
        multiAsset.insert(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ScriptHash.from_bytes(fromHex(policy)), assetsValue);
    });
    const value = _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.Value["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.BigNum.from_str(lovelace ? lovelace.toString() : "0"));
    if (units.length > 1 || !lovelace)
        value.set_multiasset(multiAsset);
    return value;
}
function fromScriptRef(scriptRef) {
    const kind = scriptRef.get().kind();
    switch (kind) {
        case 0:
            return {
                type: "Native",
                script: toHex(scriptRef.get().as_native().to_bytes()),
            };
        case 1:
            return {
                type: "PlutusV1",
                script: toHex(scriptRef.get().as_plutus_v1().to_bytes()),
            };
        case 2:
            return {
                type: "PlutusV2",
                script: toHex(scriptRef.get().as_plutus_v2().to_bytes()),
            };
        default:
            throw new Error("No variant matched.");
    }
}
function toScriptRef(script) {
    switch (script.type) {
        case "Native":
            return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ScriptRef["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.Script.new_native(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.NativeScript.from_bytes(fromHex(script.script))));
        case "PlutusV1":
            return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ScriptRef["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.Script.new_plutus_v1(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusScript.from_bytes(fromHex(applyDoubleCborEncoding(script.script)))));
        case "PlutusV2":
            return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ScriptRef["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.Script.new_plutus_v2(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusScript.from_bytes(fromHex(applyDoubleCborEncoding(script.script)))));
        default:
            throw new Error("No variant matched.");
    }
}
function utxoToCore(utxo) {
    const address = (() => {
        try {
            return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.Address.from_bech32(utxo.address);
        }
        catch (_e) {
            return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ByronAddress.from_base58(utxo.address).to_address();
        }
    })();
    const output = _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.TransactionOutput["new"](address, assetsToValue(utxo.assets));
    if (utxo.datumHash) {
        output.set_datum(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.Datum.new_data_hash(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.DataHash.from_bytes(fromHex(utxo.datumHash))));
    }
    // inline datum
    if (!utxo.datumHash && utxo.datum) {
        output.set_datum(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.Datum.new_data(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.Data["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusData.from_bytes(fromHex(utxo.datum)))));
    }
    if (utxo.scriptRef) {
        output.set_script_ref(toScriptRef(utxo.scriptRef));
    }
    return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.TransactionUnspentOutput["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.TransactionInput["new"](_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.TransactionHash.from_bytes(fromHex(utxo.txHash)), _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.BigNum.from_str(utxo.outputIndex.toString())), output);
}
function coreToUtxo(coreUtxo) {
    return {
        txHash: toHex(coreUtxo.input().transaction_id().to_bytes()),
        outputIndex: parseInt(coreUtxo.input().index().to_str()),
        assets: valueToAssets(coreUtxo.output().amount()),
        address: coreUtxo.output().address().as_byron()
            ? coreUtxo.output().address().as_byron()?.to_base58()
            : coreUtxo.output().address().to_bech32(undefined),
        datumHash: coreUtxo.output()?.datum()?.as_data_hash()?.to_hex(),
        datum: coreUtxo.output()?.datum()?.as_data() &&
            toHex(coreUtxo.output().datum().as_data().get().to_bytes()),
        scriptRef: coreUtxo.output()?.script_ref() &&
            fromScriptRef(coreUtxo.output().script_ref()),
    };
}
function networkToId(network) {
    switch (network) {
        case "Preview":
            return 0;
        case "Preprod":
            return 0;
        case "Custom":
            return 0;
        case "Mainnet":
            return 1;
        default:
            throw new Error("Network not found");
    }
}
function fromHex(hex) {
    return (0,_deps_deno_land_std_0_100_0_encoding_hex_js__WEBPACK_IMPORTED_MODULE_0__.decodeString)(hex);
}
function toHex(bytes) {
    return (0,_deps_deno_land_std_0_100_0_encoding_hex_js__WEBPACK_IMPORTED_MODULE_0__.encodeToString)(bytes);
}
/** Convert a Hex encoded string to a Utf-8 encoded string. */
function toText(hex) {
    return new TextDecoder().decode((0,_deps_deno_land_std_0_100_0_encoding_hex_js__WEBPACK_IMPORTED_MODULE_0__.decode)(new TextEncoder().encode(hex)));
}
/** Convert a Utf-8 encoded string to a Hex encoded string. */
function fromText(text) {
    return toHex(new TextEncoder().encode(text));
}
function toPublicKey(privateKey) {
    return _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PrivateKey.from_bech32(privateKey).to_public().to_bech32();
}
/** Padded number in Hex. */
function checksum(num) {
    return (0,_misc_crc8_js__WEBPACK_IMPORTED_MODULE_3__.crc8)(fromHex(num)).toString(16).padStart(2, "0");
}
function toLabel(num) {
    if (num < 0 || num > 65535) {
        throw new Error(`Label ${num} out of range: min label 1 - max label 65535.`);
    }
    const numHex = num.toString(16).padStart(4, "0");
    return "0" + numHex + checksum(numHex) + "0";
}
function fromLabel(label) {
    if (label.length !== 8 || !(label[0] === "0" && label[7] === "0")) {
        return null;
    }
    const numHex = label.slice(1, 5);
    const num = parseInt(numHex, 16);
    const check = label.slice(5, 7);
    return check === checksum(numHex) ? num : null;
}
/**
 * @param name Hex encoded
 */
function toUnit(policyId, name, label) {
    const hexLabel = Number.isInteger(label) ? toLabel(label) : "";
    const n = name ? name : "";
    if ((n + hexLabel).length > 64) {
        throw new Error("Asset name size exceeds 32 bytes.");
    }
    if (policyId.length !== 56) {
        throw new Error(`Policy id invalid: ${policyId}.`);
    }
    return policyId + hexLabel + n;
}
/**
 * Splits unit into policy id, asset name (entire asset name), name (asset name without label) and label if applicable.
 * name will be returned in Hex.
 */
function fromUnit(unit) {
    const policyId = unit.slice(0, 56);
    const assetName = unit.slice(56) || null;
    const label = fromLabel(unit.slice(56, 64));
    const name = (() => {
        const hexName = Number.isInteger(label) ? unit.slice(64) : unit.slice(56);
        return hexName || null;
    })();
    return { policyId, assetName, name, label };
}
/**
 * Convert a native script from Json to the Hex representation.
 * It follows this Json format: https://github.com/input-output-hk/cardano-node/blob/master/doc/reference/simple-scripts.md
 */
function nativeScriptFromJson(nativeScript) {
    return {
        type: "Native",
        script: toHex(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.encode_json_str_to_native_script(JSON.stringify(nativeScript), "", _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.ScriptSchema.Node).to_bytes()),
    };
}
function applyParamsToScript(plutusScript, params, shape) {
    const p = (shape ? _plutus_data_js__WEBPACK_IMPORTED_MODULE_5__.Data.castTo(params, shape) : params);
    return toHex(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.apply_params_to_plutus_script(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusList.from_bytes(fromHex(_plutus_data_js__WEBPACK_IMPORTED_MODULE_5__.Data.to(p))), _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusScript.from_bytes(fromHex(applyDoubleCborEncoding(plutusScript)))).to_bytes());
}
/** Returns double cbor encoded script. If script is already double cbor encoded it's returned as it is. */
function applyDoubleCborEncoding(script) {
    try {
        _core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusScript.from_bytes(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusScript.from_bytes(fromHex(script)).bytes());
        return script;
    }
    catch (_e) {
        return toHex(_core_mod_js__WEBPACK_IMPORTED_MODULE_1__.C.PlutusScript["new"](fromHex(script)).to_bytes());
    }
}

__webpack_async_result__();
} catch(e) { __webpack_async_result__(e); } });

/***/ })

/******/ 	});
/************************************************************************/
/******/ 	// The module cache
/******/ 	var __webpack_module_cache__ = {};
/******/ 	
/******/ 	// The require function
/******/ 	function __webpack_require__(moduleId) {
/******/ 		// Check if module is in cache
/******/ 		var cachedModule = __webpack_module_cache__[moduleId];
/******/ 		if (cachedModule !== undefined) {
/******/ 			return cachedModule.exports;
/******/ 		}
/******/ 		// Create a new module (and put it into the cache)
/******/ 		var module = __webpack_module_cache__[moduleId] = {
/******/ 			// no module.id needed
/******/ 			// no module.loaded needed
/******/ 			exports: {}
/******/ 		};
/******/ 	
/******/ 		// Execute the module function
/******/ 		__webpack_modules__[moduleId](module, module.exports, __webpack_require__);
/******/ 	
/******/ 		// Return the exports of the module
/******/ 		return module.exports;
/******/ 	}
/******/ 	
/******/ 	// expose the modules object (__webpack_modules__)
/******/ 	__webpack_require__.m = __webpack_modules__;
/******/ 	
/************************************************************************/
/******/ 	/* webpack/runtime/async module */
/******/ 	(() => {
/******/ 		var webpackQueues = typeof Symbol === "function" ? Symbol("webpack queues") : "__webpack_queues__";
/******/ 		var webpackExports = typeof Symbol === "function" ? Symbol("webpack exports") : "__webpack_exports__";
/******/ 		var webpackError = typeof Symbol === "function" ? Symbol("webpack error") : "__webpack_error__";
/******/ 		var resolveQueue = (queue) => {
/******/ 			if(queue && !queue.d) {
/******/ 				queue.d = 1;
/******/ 				queue.forEach((fn) => (fn.r--));
/******/ 				queue.forEach((fn) => (fn.r-- ? fn.r++ : fn()));
/******/ 			}
/******/ 		}
/******/ 		var wrapDeps = (deps) => (deps.map((dep) => {
/******/ 			if(dep !== null && typeof dep === "object") {
/******/ 				if(dep[webpackQueues]) return dep;
/******/ 				if(dep.then) {
/******/ 					var queue = [];
/******/ 					queue.d = 0;
/******/ 					dep.then((r) => {
/******/ 						obj[webpackExports] = r;
/******/ 						resolveQueue(queue);
/******/ 					}, (e) => {
/******/ 						obj[webpackError] = e;
/******/ 						resolveQueue(queue);
/******/ 					});
/******/ 					var obj = {};
/******/ 					obj[webpackQueues] = (fn) => (fn(queue));
/******/ 					return obj;
/******/ 				}
/******/ 			}
/******/ 			var ret = {};
/******/ 			ret[webpackQueues] = x => {};
/******/ 			ret[webpackExports] = dep;
/******/ 			return ret;
/******/ 		}));
/******/ 		__webpack_require__.a = (module, body, hasAwait) => {
/******/ 			var queue;
/******/ 			hasAwait && ((queue = []).d = 1);
/******/ 			var depQueues = new Set();
/******/ 			var exports = module.exports;
/******/ 			var currentDeps;
/******/ 			var outerResolve;
/******/ 			var reject;
/******/ 			var promise = new Promise((resolve, rej) => {
/******/ 				reject = rej;
/******/ 				outerResolve = resolve;
/******/ 			});
/******/ 			promise[webpackExports] = exports;
/******/ 			promise[webpackQueues] = (fn) => (queue && fn(queue), depQueues.forEach(fn), promise["catch"](x => {}));
/******/ 			module.exports = promise;
/******/ 			body((deps) => {
/******/ 				currentDeps = wrapDeps(deps);
/******/ 				var fn;
/******/ 				var getResult = () => (currentDeps.map((d) => {
/******/ 					if(d[webpackError]) throw d[webpackError];
/******/ 					return d[webpackExports];
/******/ 				}))
/******/ 				var promise = new Promise((resolve) => {
/******/ 					fn = () => (resolve(getResult));
/******/ 					fn.r = 0;
/******/ 					var fnQueue = (q) => (q !== queue && !depQueues.has(q) && (depQueues.add(q), q && !q.d && (fn.r++, q.push(fn))));
/******/ 					currentDeps.map((dep) => (dep[webpackQueues](fnQueue)));
/******/ 				});
/******/ 				return fn.r ? promise : getResult();
/******/ 			}, (err) => ((err ? reject(promise[webpackError] = err) : outerResolve(exports)), resolveQueue(queue)));
/******/ 			queue && (queue.d = 0);
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/define property getters */
/******/ 	(() => {
/******/ 		// define getter functions for harmony exports
/******/ 		__webpack_require__.d = (exports, definition) => {
/******/ 			for(var key in definition) {
/******/ 				if(__webpack_require__.o(definition, key) && !__webpack_require__.o(exports, key)) {
/******/ 					Object.defineProperty(exports, key, { enumerable: true, get: definition[key] });
/******/ 				}
/******/ 			}
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/ensure chunk */
/******/ 	(() => {
/******/ 		__webpack_require__.f = {};
/******/ 		// This file contains only the entry chunk.
/******/ 		// The chunk loading function for additional chunks
/******/ 		__webpack_require__.e = (chunkId) => {
/******/ 			return Promise.all(Object.keys(__webpack_require__.f).reduce((promises, key) => {
/******/ 				__webpack_require__.f[key](chunkId, promises);
/******/ 				return promises;
/******/ 			}, []));
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/get javascript chunk filename */
/******/ 	(() => {
/******/ 		// This function allow to reference async chunks
/******/ 		__webpack_require__.u = (chunkId) => {
/******/ 			// return url for filenames based on template
/******/ 			return "" + chunkId + ".app.bundle.js";
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/global */
/******/ 	(() => {
/******/ 		__webpack_require__.g = (function() {
/******/ 			if (typeof globalThis === 'object') return globalThis;
/******/ 			try {
/******/ 				return this || new Function('return this')();
/******/ 			} catch (e) {
/******/ 				if (typeof window === 'object') return window;
/******/ 			}
/******/ 		})();
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/hasOwnProperty shorthand */
/******/ 	(() => {
/******/ 		__webpack_require__.o = (obj, prop) => (Object.prototype.hasOwnProperty.call(obj, prop))
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/load script */
/******/ 	(() => {
/******/ 		var inProgress = {};
/******/ 		var dataWebpackPrefix = "teddyswap.ui:";
/******/ 		// loadScript function to load a script via script tag
/******/ 		__webpack_require__.l = (url, done, key, chunkId) => {
/******/ 			if(inProgress[url]) { inProgress[url].push(done); return; }
/******/ 			var script, needAttach;
/******/ 			if(key !== undefined) {
/******/ 				var scripts = document.getElementsByTagName("script");
/******/ 				for(var i = 0; i < scripts.length; i++) {
/******/ 					var s = scripts[i];
/******/ 					if(s.getAttribute("src") == url || s.getAttribute("data-webpack") == dataWebpackPrefix + key) { script = s; break; }
/******/ 				}
/******/ 			}
/******/ 			if(!script) {
/******/ 				needAttach = true;
/******/ 				script = document.createElement('script');
/******/ 		
/******/ 				script.charset = 'utf-8';
/******/ 				script.timeout = 120;
/******/ 				if (__webpack_require__.nc) {
/******/ 					script.setAttribute("nonce", __webpack_require__.nc);
/******/ 				}
/******/ 				script.setAttribute("data-webpack", dataWebpackPrefix + key);
/******/ 				script.src = url;
/******/ 			}
/******/ 			inProgress[url] = [done];
/******/ 			var onScriptComplete = (prev, event) => {
/******/ 				// avoid mem leaks in IE.
/******/ 				script.onerror = script.onload = null;
/******/ 				clearTimeout(timeout);
/******/ 				var doneFns = inProgress[url];
/******/ 				delete inProgress[url];
/******/ 				script.parentNode && script.parentNode.removeChild(script);
/******/ 				doneFns && doneFns.forEach((fn) => (fn(event)));
/******/ 				if(prev) return prev(event);
/******/ 			};
/******/ 			var timeout = setTimeout(onScriptComplete.bind(null, undefined, { type: 'timeout', target: script }), 120000);
/******/ 			script.onerror = onScriptComplete.bind(null, script.onerror);
/******/ 			script.onload = onScriptComplete.bind(null, script.onload);
/******/ 			needAttach && document.head.appendChild(script);
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/make namespace object */
/******/ 	(() => {
/******/ 		// define __esModule on exports
/******/ 		__webpack_require__.r = (exports) => {
/******/ 			if(typeof Symbol !== 'undefined' && Symbol.toStringTag) {
/******/ 				Object.defineProperty(exports, Symbol.toStringTag, { value: 'Module' });
/******/ 			}
/******/ 			Object.defineProperty(exports, '__esModule', { value: true });
/******/ 		};
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/publicPath */
/******/ 	(() => {
/******/ 		var scriptUrl;
/******/ 		if (__webpack_require__.g.importScripts) scriptUrl = __webpack_require__.g.location + "";
/******/ 		var document = __webpack_require__.g.document;
/******/ 		if (!scriptUrl && document) {
/******/ 			if (document.currentScript)
/******/ 				scriptUrl = document.currentScript.src
/******/ 			if (!scriptUrl) {
/******/ 				var scripts = document.getElementsByTagName("script");
/******/ 				if(scripts.length) scriptUrl = scripts[scripts.length - 1].src
/******/ 			}
/******/ 		}
/******/ 		// When supporting browsers where an automatic publicPath is not supported you must specify an output.publicPath manually via configuration
/******/ 		// or pass an empty string ("") and set the __webpack_public_path__ variable from your code to use your own logic.
/******/ 		if (!scriptUrl) throw new Error("Automatic publicPath is not supported in this browser");
/******/ 		scriptUrl = scriptUrl.replace(/#.*$/, "").replace(/\?.*$/, "").replace(/\/[^\/]+$/, "/");
/******/ 		__webpack_require__.p = scriptUrl;
/******/ 	})();
/******/ 	
/******/ 	/* webpack/runtime/jsonp chunk loading */
/******/ 	(() => {
/******/ 		__webpack_require__.b = document.baseURI || self.location.href;
/******/ 		
/******/ 		// object to store loaded and loading chunks
/******/ 		// undefined = chunk not loaded, null = chunk preloaded/prefetched
/******/ 		// [resolve, reject, Promise] = chunk loading, 0 = chunk loaded
/******/ 		var installedChunks = {
/******/ 			"main": 0
/******/ 		};
/******/ 		
/******/ 		__webpack_require__.f.j = (chunkId, promises) => {
/******/ 				// JSONP chunk loading for javascript
/******/ 				var installedChunkData = __webpack_require__.o(installedChunks, chunkId) ? installedChunks[chunkId] : undefined;
/******/ 				if(installedChunkData !== 0) { // 0 means "already installed".
/******/ 		
/******/ 					// a Promise means "currently loading".
/******/ 					if(installedChunkData) {
/******/ 						promises.push(installedChunkData[2]);
/******/ 					} else {
/******/ 						if(true) { // all chunks have JS
/******/ 							// setup Promise in chunk cache
/******/ 							var promise = new Promise((resolve, reject) => (installedChunkData = installedChunks[chunkId] = [resolve, reject]));
/******/ 							promises.push(installedChunkData[2] = promise);
/******/ 		
/******/ 							// start chunk loading
/******/ 							var url = __webpack_require__.p + __webpack_require__.u(chunkId);
/******/ 							// create error before stack unwound to get useful stacktrace later
/******/ 							var error = new Error();
/******/ 							var loadingEnded = (event) => {
/******/ 								if(__webpack_require__.o(installedChunks, chunkId)) {
/******/ 									installedChunkData = installedChunks[chunkId];
/******/ 									if(installedChunkData !== 0) installedChunks[chunkId] = undefined;
/******/ 									if(installedChunkData) {
/******/ 										var errorType = event && (event.type === 'load' ? 'missing' : event.type);
/******/ 										var realSrc = event && event.target && event.target.src;
/******/ 										error.message = 'Loading chunk ' + chunkId + ' failed.\n(' + errorType + ': ' + realSrc + ')';
/******/ 										error.name = 'ChunkLoadError';
/******/ 										error.type = errorType;
/******/ 										error.request = realSrc;
/******/ 										installedChunkData[1](error);
/******/ 									}
/******/ 								}
/******/ 							};
/******/ 							__webpack_require__.l(url, loadingEnded, "chunk-" + chunkId, chunkId);
/******/ 						} else installedChunks[chunkId] = 0;
/******/ 					}
/******/ 				}
/******/ 		};
/******/ 		
/******/ 		// no prefetching
/******/ 		
/******/ 		// no preloaded
/******/ 		
/******/ 		// no HMR
/******/ 		
/******/ 		// no HMR manifest
/******/ 		
/******/ 		// no on chunks loaded
/******/ 		
/******/ 		// install a JSONP callback for chunk loading
/******/ 		var webpackJsonpCallback = (parentChunkLoadingFunction, data) => {
/******/ 			var [chunkIds, moreModules, runtime] = data;
/******/ 			// add "moreModules" to the modules object,
/******/ 			// then flag all "chunkIds" as loaded and fire callback
/******/ 			var moduleId, chunkId, i = 0;
/******/ 			if(chunkIds.some((id) => (installedChunks[id] !== 0))) {
/******/ 				for(moduleId in moreModules) {
/******/ 					if(__webpack_require__.o(moreModules, moduleId)) {
/******/ 						__webpack_require__.m[moduleId] = moreModules[moduleId];
/******/ 					}
/******/ 				}
/******/ 				if(runtime) var result = runtime(__webpack_require__);
/******/ 			}
/******/ 			if(parentChunkLoadingFunction) parentChunkLoadingFunction(data);
/******/ 			for(;i < chunkIds.length; i++) {
/******/ 				chunkId = chunkIds[i];
/******/ 				if(__webpack_require__.o(installedChunks, chunkId) && installedChunks[chunkId]) {
/******/ 					installedChunks[chunkId][0]();
/******/ 				}
/******/ 				installedChunks[chunkId] = 0;
/******/ 			}
/******/ 		
/******/ 		}
/******/ 		
/******/ 		var chunkLoadingGlobal = self["webpackChunkteddyswap_ui"] = self["webpackChunkteddyswap_ui"] || [];
/******/ 		chunkLoadingGlobal.forEach(webpackJsonpCallback.bind(null, 0));
/******/ 		chunkLoadingGlobal.push = webpackJsonpCallback.bind(null, chunkLoadingGlobal.push.bind(chunkLoadingGlobal));
/******/ 	})();
/******/ 	
/************************************************************************/
/******/ 	
/******/ 	// startup
/******/ 	// Load entry module and return exports
/******/ 	// This entry module used 'module' so it can't be inlined
/******/ 	var __webpack_exports__ = __webpack_require__("./wwwroot/scripts/App.ts");
/******/ 	
/******/ })()
;