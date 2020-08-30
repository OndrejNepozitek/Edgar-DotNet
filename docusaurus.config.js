const versions = require('./versions.json');
const getBookmarks = require("./src/bookmarks")
const [latestVersion] = require('./versions.json');
const remarkBookmarks = require('remark-bookmarks')
const path = require('path');

module.exports = {
  title: "Edgar for .NET",
  tagline:
    "Library for procedural generation of 2D levels based on a graph of room connections.",
  url: "https://ondrejnepozitek.github.io",
  baseUrl: "/Edgar-DotNet/",
  favicon: "img/favicon.ico",
  organizationName: "OndrejNepozitek", // Usually your GitHub org/user name.
  projectName: "Edgar-DotNet", // Usually your repo name.
  onBrokenLinks: "warn",
  themeConfig: {
    sidebarCollapsible: false,
    navbar: {
      title: "Edgar-DotNet",
      items: [
        { to: "versions", label: `v${latestVersion}`, position: "left" },
        {
          label: 'Docs',
          to: 'docs', // "fake" link
          position: 'right',
          activeBaseRegex: `docs/(?!next/(support|team|resources))`,
          items: [
            {
              label: versions[0],
              to: 'docs/introduction',
              activeBaseRegex: `docs/(?!${versions.join('|')}|next)`,
            },
            ...versions.slice(1).map((version) => ({
              label: version,
              to: `docs/${version}/introduction`,
            })),
            {
              label: 'Master/Unreleased',
              to: 'docs/next/introduction',
              activeBaseRegex: `docs/next/(?!support|team|resources)`,
            },
          ],
        },
        {
          href: "https://github.com/OndrejNepozitek/Edgar-DotNet/",
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
              href: "https://github.com/OndrejNepozitek/Edgar-DotNet"
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
          editUrl: "https://github.com/OndrejNepozitek/Edgar-DotNet/tree/docusaurus",
          remarkPlugins: [
            [
              remarkBookmarks, { 
                bookmarks: getBookmarks(),
              }
            ]
          ],
        },
        theme: {
          customCss: require.resolve("./src/css/custom.css")
        }
      }
    ]
  ]
};
