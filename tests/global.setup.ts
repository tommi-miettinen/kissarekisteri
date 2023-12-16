import { chromium, type FullConfig } from "@playwright/test";

async function globalSetup(config: FullConfig) {
  const { storageState } = config.projects[0].use;
  const browser = await chromium.launch({ headless: false, slowMo: 50 });
  const context = await browser.newContext({ ignoreHTTPSErrors: true });
  const page = await context.newPage();

  await page.goto("https://localhost:5173");

  await page.getByTestId("login-btn").click();

  await page.locator("id=email").fill("tommi.a.miettinen@gmail.com");
  await page.locator("id=password").fill("testiukkeli_123");
  await page.locator("id=next").click();

  await page.waitForURL("https://localhost:5173/cats");
  await page.context().storageState({ path: storageState as string });
  await browser.close();
}

export default globalSetup;
