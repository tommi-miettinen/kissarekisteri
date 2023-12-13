import { test, expect } from "@playwright/test";

test("has title", async ({ page }) => {
  await page.goto("http://localhost:5173/cats");

  const h3 = page.getByRole("heading").getByText("Kissat");

  await expect(h3).toBeAttached();
});
