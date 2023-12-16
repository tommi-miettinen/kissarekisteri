import { test, expect, chromium, BrowserContext, Browser, Page } from "@playwright/test";

let browser: Browser;
let context: BrowserContext;
let page: Page;

test.beforeAll(async () => {
  browser = await chromium.launch({ headless: false, slowMo: 200 });
  context = await browser.newContext({ ignoreHTTPSErrors: true });
  page = await context.newPage();
});

test.afterAll(async () => {});

test("cat creation works", async () => {
  await page.goto("https://localhost:5173/profile");

  await page.waitForTimeout(2000);

  await page.getByTestId("add-new-cat-btn").click();
  await page.getByTestId("new-cat-name-input").fill("testikissa");
  await page.getByTestId("new-cat-breed-input").fill("testirotu");
  await page.getByTestId("new-cat-birthdate-input").fill("2023-12-22");
  await page.getByTestId("add-new-cat-btn-save").click();

  await page.reload();

  const testikissa = page.getByText("testikissa");

  expect(testikissa).toBeTruthy();
});

test("cat searching works", async () => {
  await page.goto("https://localhost:5173/cats");

  const catName = "testikissa";

  const catSearchInput = page.getByTestId("cat-search-input");
  await catSearchInput.fill(catName);
  const filteredCat = page.getByText(catName);
  expect(filteredCat).toBeTruthy();

  const inputThatDoesntMatch = Math.random().toString();
  await catSearchInput.fill(inputThatDoesntMatch);

  const noCatsFound = page.locator(".cat");
  const noCatsCount = await noCatsFound.count();
  expect(noCatsCount).toBe(0);
});

test("cat deletion works", async () => {
  await page.goto("https://localhost:5173/profile");

  const catName = "testikissa";
  const catElement = page.locator(".cat").first();

  await catElement.getByTestId("cat-options").click();
  await catElement.getByTestId("start-cat-delete").click();
  await page.getByTestId("confirm-cat-delete").click();

  await page.reload({ waitUntil: "load" });

  const catIsGone = (await page.locator(`text=${catName}`).count()) === 0;
  await page.waitForTimeout(5000);
  expect(catIsGone).toBeTruthy();
});

/*


test("cat editing works", async () => {
  await page.goto("https://localhost:5173/profile");

  await page.waitForTimeout(2000);

  await page.getByTestId("add-new-cat-btn").click();
  await page.getByTestId("new-cat-name-input").fill("testikissa");
  await page.getByTestId("new-cat-breed-input").fill("testirotu");
  await page.getByTestId("new-cat-birthdate-input").fill("2023-12-22");
  await page.getByTestId("add-new-cat-btn-save").click();

  await page.reload();

  const testikissa = await page.getByText("testikissa");

  expect(testikissa).toBeTruthy();
});



test("cat deletion works", async () => {
  await page.goto("https://localhost:5173/profile");

  await page.waitForTimeout(2000);

  await page.getByTestId("add-new-cat-btn").click();
  await page.getByTestId("new-cat-name-input").fill("testikissa");
  await page.getByTestId("new-cat-breed-input").fill("testirotu");
  await page.getByTestId("new-cat-birthdate-input").fill("2023-12-22");
  await page.getByTestId("add-new-cat-btn-save").click();

  await page.reload();

  const testikissa = await page.getByText("testikissa");

  expect(testikissa).toBeTruthy();
});

*/
