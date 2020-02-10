module.exports = {
  title: "Procedural level generator",
  tagline:
    "Library for procedural generation of 2D levels based on a graph of room connections.",
  url: "https://ondrejnepozitek.github.io",
  baseUrl: "/ProceduralLevelGenerator/",
  favicon: "img/favicon.ico",
  organizationName: "OndrejNepozitek", // Usually your GitHub org/user name.
  projectName: "ProceduralLevelGenerator", // Usually your repo name.
  themeConfig: {
    navbar: {
      title: "Procedural level generator",
      links: [
        { to: "versions", label: "v1.0.6", position: "left" },
        { to: "docs/introduction", label: "Docs", position: "right" },
        {
          href: "https://github.com/OndrejNepozitek/ProceduralLevelGenerator/",
          label: "GitHub",
          position: "right"
        }
      ]
    },
    prism: {
      defaultLanguage: "csharp",
      theme: require("./src/theme/prism-darcula")
    },
    footer: {
      style: "dark",
      links: [
        {
          title: "Docs",
          items: [
            {
              label: "Introduction",
              to: "docs/introduction"
            },
          ]
        },
        {
          title: "Community",
          items: [
            {
              label: "Twitter",
              href: "https://twitter.com/OndrejNepozitek"
            },
          ]
        },
        {
          title: "Social",
          items: [
            {
              label: "Blog",
              href: "https://ondra.nepozitek.cz/blog/"
            },
            {
              label: "GitHub",
              href: "https://github.com/OndrejNepozitek/ProceduralLevelGenerator"
            },
          ]
        }
      ],
      copyright: "Copyright © " + new Date().getFullYear() + " Ondřej Nepožitek"
    }
  },
  presets: [
    [
      "@docusaurus/preset-classic",
      {
        docs: {
          sidebarPath: require.resolve("./sidebars.js"),
          editUrl: "https://github.com/OndrejNepozitek/ProceduralLevelGenerator/tree/docusaurus"
        },
        theme: {
          customCss: require.resolve("./src/css/custom.css")
        }
      }
    ]
  ]
};
