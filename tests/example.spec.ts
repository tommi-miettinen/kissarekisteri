import { test, expect, chromium } from "@playwright/test";

let browser;
let context;
let page;

test.beforeAll(async () => {
  browser = await chromium.launch({ headless: false, slowMo: 50 });
  context = await browser.newContext({ ignoreHTTPSErrors: true });
  page = await context.newPage();
});

test.afterAll(async () => {});

test("login works", async () => {
  await page.goto("https://localhost:5173");

  await page.getByTestId("login-btn").click();

  await page.locator("id=email").fill("tommi.a.miettinen@gmail.com");
  await page.locator("id=password").fill("testiukkeli_123");
  await page.locator("id=next").click();

  await page.waitForURL("https://localhost:5173/cats");

  expect(page.url()).toBe("https://localhost:5173/cats");
});

test("cat creation works", async () => {
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
