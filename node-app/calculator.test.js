const test = require('node:test');
const assert = require('node:assert');
const { add } = require('./calculator');

test('add iki sayiyi toplar', () => {
  assert.strictEqual(add(2, 3), 5);
});
