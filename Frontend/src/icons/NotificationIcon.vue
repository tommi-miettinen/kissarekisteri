<script lang="ts" setup>
defineProps({
  size: {
    type: [String, Number],
    default: 24,
  },
  color: {
    type: String,
    default: "currentColor",
  },
  strokeWidth: {
    type: [String, Number],
    default: 3,
  },
  customClass: {
    type: String,
    default: "",
  },
});
</script>

<template>
  <svg
    xmlns="http://www.w3.org/2000/svg"
    width="20"
    height="20"
    fill="none"
    stroke="#000"
    strokeLinecap="round"
    strokeLinejoin="round"
    :strokeWidth="strokeWidth"
    className="feather feather-bell"
    viewBox="0 0 24 24"
  >
    <path d="M18 8A6 6 0 006 8c0 7-3 9-3 9h18s-3-2-3-9M13.73 21a2 2 0 01-3.46 0"></path>
  </svg>
</template>
